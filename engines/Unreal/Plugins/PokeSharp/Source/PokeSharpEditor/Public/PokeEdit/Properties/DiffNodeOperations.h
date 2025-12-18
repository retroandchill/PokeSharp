// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PokeEdit/Schema/DiffNode.h"
#include "PokeEdit/Serialization/JsonSchema.h"
#include "Templates/Overload.h"

inline bool operator==(const FText &A, const FText &B)
{
    return A.EqualTo(B);
}

namespace PokeEdit
{

    template <TValidJsonObjectContainer T>
    auto GetPropertyLookup()
    {
        using FMembers = TMemberVariant<decltype(TJsonObjectSchema<T>)>;
        TMap<FName, FMembers> Result;
        TJsonObjectSchema<T>.ForEachField(
            [&]<auto Member>(const TJsonField<Member> &Field)
            { Result.Add(FName(Field.CppName), FMembers(TInPlaceType<TJsonField<Member>>{}, Field)); });
        return Result;
    }

    template <typename T>
        requires TValidJsonObjectContainer<T> || TJsonDeserializable<T>
    std::expected<void, FString> ApplyEdit(T &Value, const FDiffNode &DiffNode)
    {
        if (auto *Set = DiffNode.TryGet<FValueSetNode>(); Set != nullptr)
        {
            return DeserializeFromJson<T>(Set->NewValue)
                .transform([&Value](T &&ParsedValue) { Value = MoveTemp(ParsedValue); });
        }

        if constexpr (TValidJsonObjectContainer<T>)
        {
            auto *ObjectDiff = DiffNode.TryGet<FObjectDiffNode>();
            if (ObjectDiff == nullptr)
            {
                return std::unexpected(TEXT("Invalid traversal type for an object"));
            }

            return ApplyEdit<T>(Value, *ObjectDiff);
        }
        else
        {
            return std::unexpected(TEXT("Cannot traverse into a scalar value"));
        }
    }

    template <typename T>
        requires TValidJsonObjectContainer<T> || TJsonDeserializable<T>
    std::expected<void, FString> ApplyEdit(T &Value, const FObjectDiffNode &ObjectDiff)
    {
        static const auto FieldLookup = GetPropertyLookup<T>();
        auto &ObjectRef = TJsonObjectContainer<T>::GetMutableObjectRef(Value);
        for (const auto &[FieldName, Field] : ObjectDiff.Properties)
        {
            const auto *FieldEdit = FieldLookup.Find(FieldName);
            if (FieldEdit == nullptr)
            {
                return std::unexpected(
                    FString::Format(TEXT("Field '{0}' does not exist in object"), {FieldName.ToString()}));
            }

            return Visit(
                [&]<auto Member>(const TJsonField<Member> &JsonField) -> std::expected<void, FString>
                {
                    auto &ValueReference = JsonField.GetMember(ObjectRef);

                    if (auto ApplicationResult = ApplyEdit(ValueReference, Field.Get()); !ApplicationResult.has_value())
                    {
                        return std::unexpected(MoveTemp(ApplicationResult).error());
                    }

                    return {};
                },
                *FieldEdit);
        }

        return {};
    }

    template <typename T>
    concept TCanApplyDiff = requires(T &Value, const FDiffNode &DiffNode) {
        { ApplyEdit(Value, DiffNode) } -> std::convertible_to<std::expected<void, FString>>;
    };

    template <TCanApplyDiff T>
    std::expected<void, FString> ApplyEdit(TOptional<T> &Value, const FDiffNode &DiffNode)
    {
        if (auto *Reset = DiffNode.TryGet<FValueResetNode>(); Reset != nullptr)
        {
            Value.Reset();
            return {};
        }

        if (!Value.IsSet())
        {
            return std::unexpected(TEXT("Cannot apply diff to an unset optional"));
        }

        return ApplyEdit(*Value, DiffNode);
    }

    template <TCanApplyDiff T>
    std::expected<void, FString> ApplyEdit(TArray<T> &Value, const FDiffNode &DiffNode)
    {
        return DiffNode.Visit(UE::Overload(
            [&Value](const FValueSetNode &Node)
            {
                return DeserializeFromJson<T>(Node.NewValue)
                    .transform([&Value](T &&DeserializedValue) { Value.Emplace(MoveTemp(DeserializedValue)); });
            },
            [&Value](const FListDiffNode &ListDiffNode) { return ApplyEdit(Value, ListDiffNode); },
            [](auto &&) -> std::expected<void, FString> { return std::unexpected(TEXT("Invalid diff node type")); }));
    }

    template <TCanApplyDiff T>
    std::expected<void, FString> ApplyEdit(TArray<T> &Value, const FListDiffNode &DiffNode)
    {
        for (const auto &Edit : DiffNode.Edits)
        {
            auto Result = Edit.Visit(UE::Overload(
                [&Value](const FListSetNode &SetNode) -> std::expected<void, FString>
                {
                    if (!Value.IsValidIndex(SetNode.Index))
                    {
                        return std::unexpected(FString::Printf(TEXT("Index %d out of bounds"), SetNode.Index));
                    }

                    return ApplyEdit(Value[SetNode.Index], SetNode.Change);
                },
                [&Value](const FListAddNode &AddNode)
                {
                    return DeserializeFromJson<T>(AddNode.NewValue)
                        .transform([&Value](T &&DeserializedValue) { Value.Emplace(MoveTemp(DeserializedValue)); });
                },
                [&Value](const FListInsertNode &InsertNode) -> std::expected<void, FString>
                {
                    if (InsertNode.Index < 0 && InsertNode.Index > Value.Num())
                    {
                        return std::unexpected(FString::Printf(TEXT("Index %d out of bounds"), InsertNode.Index));
                    }

                    return DeserializeFromJson<T>(InsertNode.NewValue)
                        .transform([&](T &&DeserializedValue)
                                   { Value.Insert(MoveTemp(DeserializedValue), InsertNode.Index); });
                },
                [&Value](const FListRemoveNode &RemoveNode) -> std::expected<void, FString>
                {
                    if (!Value.IsValidIndex(RemoveNode.Index))
                    {
                        return std::unexpected(FString::Printf(TEXT("Index %d out of bounds"), RemoveNode.Index));
                    }

                    Value.RemoveAt(RemoveNode.Index, 1);
                    return {};
                },
                [&Value](const FListSwapNode &SwapNode) -> std::expected<void, FString>
                {
                    if (!Value.IsValidIndex(SwapNode.IndexA))
                    {
                        return std::unexpected(FString::Printf(TEXT("Index %d out of bounds"), SwapNode.IndexA));
                    }

                    if (!Value.IsValidIndex(SwapNode.IndexB))
                    {
                        return std::unexpected(FString::Printf(TEXT("Index %d out of bounds"), SwapNode.IndexB));
                    }

                    Value.Swap(SwapNode.IndexA, SwapNode.IndexB);
                    return {};
                },
                [](auto &&) -> std::expected<void, FString>
                { return std::unexpected(TEXT("Invalid diff node type")); }));
        }
        return {};
    }

    template <TJsonDeserializable K, TCanApplyDiff V>
    std::expected<void, FString> ApplyEdit(TMap<K, V> &Value, const FDiffNode &DiffNode)
    {
        return DiffNode.Visit(UE::Overload(
            [&Value](const FValueSetNode &SetNode) -> std::expected<void, FString>
            {
                return DeserializeFromJson<TMap<K, V>>(SetNode.NewValue)
                    .transform([&Value](TMap<K, V> &&DeserializedValue) { Value = MoveTemp(DeserializedValue); });
            },
            [&Value](const FDictionaryDiffNode &DictionaryDiffNode) { return ApplyEdit(Value, DictionaryDiffNode); },
            [](auto &&) -> std::expected<void, FString> { return std::unexpected(TEXT("Invalid diff node type")); }));
    }

    template <TJsonDeserializable K, TCanApplyDiff V>
    std::expected<void, FString> ApplyEdit(TMap<K, V> &Value, const FDictionaryDiffNode &DiffNode)
    {
        for (const auto &Edit : DiffNode.Edits)
        {
            auto Result = Edit.Visit(UE::Overload(
                [&Value](const FDictionarySetNode &SetNode) -> std::expected<void, FString>
                {
                    return DeserializeFromJson<K>(SetNode.Key)
                        .and_then(
                            [&SetNode, &Value](const K &DeserializedKey) -> std::expected<void, FString>
                            {
                                auto *MapValue = Value.Find(DeserializedKey);
                                if (MapValue == nullptr)
                                {
                                    return std::unexpected(TEXT("Unable to find key in dictionary"));
                                }

                                return ApplyEdit(*MapValue, SetNode.Change);
                            });
                },
                [&Value](const FDictionaryAddNode &AddNode) -> std::expected<void, FString>
                {
                    return DeserializeFromJson<K>(AddNode.Key)
                        .and_then(
                            [&Value, &AddNode](K &DeserializedKey) -> std::expected<void, FString>
                            {
                                if (Value.Contains(DeserializedKey))
                                {
                                    return std::unexpected(TEXT("Key already exists in dictionary"));
                                }

                                return DeserializeFromJson<V>(AddNode.Value)
                                    .transform(
                                        [&Value, &DeserializedKey](V &&DeserializedValue)
                                        { Value.Emplace(MoveTemp(DeserializedKey), MoveTemp(DeserializedValue)); });
                            });
                },
                [&Value](const FDictionaryRemoveNode &RemoveNode) -> std::expected<void, FString>
                {
                    return DeserializeFromJson<K>(RemoveNode.Key)
                        .and_then(
                            [&Value](const K &DeserializedKey) -> std::expected<void, FString>
                            {
                                if (!Value.Contains(DeserializedKey))
                                {
                                    return std::unexpected(TEXT("Key does not exist in dictionary"));
                                }

                                Value.Remove(DeserializedKey);
                                return {};
                            });
                },
                [&Value](const FDictionaryChangeKeyNode &ChangeKeyNode) -> std::expected<void, FString>
                {
                    return DeserializeFromJson<K>(ChangeKeyNode.OldKey)
                        .and_then(
                            [&Value, &ChangeKeyNode](const K &OldKey) -> std::expected<void, FString>
                            {
                                if (!Value.Contains(OldKey))
                                {
                                    return std::unexpected(TEXT("Old key does not exist in dictionary"));
                                }

                                return DeserializeFromJson<K>(ChangeKeyNode.NewKey)
                                    .transform(
                                        [&Value, &OldKey](const K &NewKey)
                                        {
                                            auto OldValue = MoveTempIfPossible(Value.FindChecked(OldKey));
                                            Value.Remove(OldKey);
                                            Value.Emplace(MoveTemp(NewKey), MoveTemp(OldValue));
                                        });
                            });
                },
                [](auto &&) -> std::expected<void, FString>
                { return std::unexpected(TEXT("Invalid diff node type")); }));
        }

        return {};
    }

    template <typename T>
        requires TValidJsonObjectContainer<T> || (std::equality_comparable<T> && TJsonSerializable<T>)
    TOptional<FDiffNode> Diff(const T &OldValue, const T &NewValue)
    {
        if constexpr (TValidJsonObjectContainer<T>)
        {
            auto &OldValueRef = TJsonObjectContainer<T>::GetObjectRef(OldValue);
            auto &NewValueRef = TJsonObjectContainer<T>::GetObjectRef(NewValue);

            TMap<FName, TSharedRef<FDiffNode>> Edits;
            TJsonObjectContainer<T>::JsonSchema.ForeEachField(
                [&]<auto Member>(const TJsonField<Member> &Field)
                {
                    auto &OldPropertyValue = Field.GetMember(OldValueRef);
                    auto &NewPropertyValue = Field.GetMember(NewValueRef);

                    if (auto DiffResult = Diff(OldPropertyValue, NewPropertyValue); DiffResult.IsSet())
                    {
                        Edits.Emplace(FName(Field.CppName), DiffResult.GetValue());
                    }
                });

            if (Edits.Num() == 0)
            {
                return NullOpt;
            }

            return TOptional<FDiffNode>(InPlace, TInPlaceType<FObjectDiffNode>{}, MoveTemp(Edits));
        }
        else
        {
            return OldValue != NewValue
                       ? TOptional<FDiffNode>(InPlace, TInPlaceType<FValueSetNode>{}, SerializeToJson(NewValue))
                       : TOptional<FDiffNode>();
        }
    }

    template <typename T>
    concept TDiffableType = requires(const T &OldValue, const T &NewValue) {
        { Diff(OldValue, NewValue) } -> std::convertible_to<TOptional<FDiffNode>>;
    };

    template <TCanApplyDiff T>
    TOptional<FDiffNode> Diff(const TOptional<T> &OldValue, const TOptional<T> &NewValue)
    {
        if (!OldValue.IsSet())
        {
            return NewValue.IsSet()
                       ? TOptional<FDiffNode>(InPlace, TInPlaceType<FValueSetNode>{}, SerializeToJson(NewValue))
                       : TOptional<FDiffNode>();
        }

        if (!NewValue.IsSet())
        {
            return TOptional<FDiffNode>(InPlace, TInPlaceType<FValueResetNode>{});
        }

        return Diff(*OldValue, *NewValue);
    }

    template <TCanApplyDiff T>
    TOptional<FDiffNode> Diff(const TArray<T> &OldValue, const TArray<T> &NewValue)
    {
        if (OldValue.GetData() == NewValue.GetData())
        {
            return NullOpt;
        }

        const int32 OldCount = OldValue.Num();
        const int32 NewCount = NewValue.Num();
        const int32 MinCount = FMath::Min(OldCount, NewCount);

        TArray<FListEditNode> Edits;
        for (int32 i = 0; i < MinCount; i++)
        {
            auto &OldItem = OldValue[i];
            auto &NewItem = NewValue[i];

            if (auto DiffResult = Diff(OldItem, NewItem); DiffResult.IsSet())
            {
                Edits.Emplace(TInPlaceType<FListSetNode>{}, i, DiffResult.GetValue());
            }
        }

        if (NewCount > OldCount)
        {
            for (int32 i = MinCount; i < NewCount; i++)
            {
                auto &NewItem = NewValue[i];

                if (i == NewCount - 1)
                {
                    Edits.Emplace(TInPlaceType<FListAddNode>{}, SerializeToJson(NewItem));
                }
                else
                {
                    Edits.Emplace(TInPlaceType<FListInsertNode>{}, i, SerializeToJson(NewItem));
                }
            }
        }
        else if (NewCount < OldCount)
        {
            for (int32 i = OldCount - 1; i >= NewCount; i--)
            {
                Edits.Emplace(TInPlaceType<FListRemoveNode>{}, i);
            }
        }

        if (Edits.Num() == 0)
        {
            return NullOpt;
        }

        return TOptional<FDiffNode>(InPlace, TInPlaceType<FListDiffNode>{}, MoveTemp(Edits));
    }

    template <TJsonSerializable K, TCanApplyDiff V>
    TOptional<FDiffNode> Diff(const TMap<K, V> &OldValue, const TMap<K, V> &NewValue)
    {
        if (&OldValue == &NewValue)
        {
            return NullOpt;
        }

        TArray<FDictionaryEditNode> Edits;
        for (auto &[Key, Value] : OldValue)
        {
            auto *Existing = NewValue.Find(Key);
            if (Existing == nullptr)
            {
                Edits.Emplace(TInPlaceType<FDictionaryRemoveNode>{}, SerializeToJson(Key));
            }
            else if (auto DiffResult = Diff(Value, *Existing); DiffResult.IsSet())
            {
                Edits.Emplace(TInPlaceType<FDictionarySetNode>{}, SerializeToJson(Key), DiffResult.GetValue());
            }
        }

        for (auto &[Key, Value] : NewValue)
        {
            if (!OldValue.Contains(Key))
            {
                Edits.Emplace(TInPlaceType<FDictionaryAddNode>{}, SerializeToJson(Key), SerializeToJson(Value));
            }
        }

        if (Edits.Num() == 0)
        {
            return NullOpt;
        }

        return TOptional<FDiffNode>(InPlace, TInPlaceType<FDictionaryDiffNode>{}, MoveTemp(Edits));
    }

} // namespace PokeEdit
