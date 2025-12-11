// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PokeEdit/Schema/FieldEdit.h"
#include "Templates/Overload.h"

namespace PokeEdit
{
    template <typename T>
    concept THasEqualityCheck = std::equality_comparable<T> || requires(const T &A, const T &B) {
        { A.EqualTo(B) } -> std::convertible_to<bool>;
    };

    template <THasEqualityCheck T>
    constexpr bool CompareEqual(const T &A, const T &B)
    {
        if constexpr (std::equality_comparable<T>)
        {
            return A == B;
        }
        else
        {
            return A.EqualTo(B);
        }
    }

    template <typename T>
    concept TDiffableType =
        requires(const T &OldValue, const T &NewValue, TArray<FFieldEdit> &Edits, const FFieldPath &BasePath) {
            CollectDiffs(OldValue, NewValue, Edits, BasePath);
        };

    template <typename T>
        requires TJsonObject<T> || (THasEqualityCheck<T> && TJsonSerializable<T>)
    void CollectDiffs(const T &OldRoot, const T &NewRoot, TArray<FFieldEdit> &Edits, const FFieldPath &BasePath)
    {
        if constexpr (TJsonObject<T>)
        {
            TJsonObjectTraits<T>::JsonSchema.ForEachField(
                [&]<auto Member>(const TJsonField<Member> &Field)
                {
                    auto &OldValue = Field.GetMember(OldRoot);
                    auto &NewValue = Field.GetMember(NewRoot);

                    auto PropertyPath = BasePath;
                    PropertyPath.Segments.Emplace(TInPlaceType<FPropertySegment>{}, FName(Field.CppName));
                    CollectDiffs(OldValue, NewValue, Edits, PropertyPath);
                });
        }
        else
        {
            if (CompareEqual(OldRoot, NewRoot))
                return;

            Edits.Emplace(TInPlaceType<FSetValueEdit>{}, BasePath, SerializeToJson(NewRoot));
        }
    }

    template <TDiffableType T>
    void CollectDiffs(const TArray<T> &OldValues,
                      const TArray<T> &NewValues,
                      TArray<FFieldEdit> &Edits,
                      const FFieldPath &BasePath)
    {
        if (&OldValues == &NewValues)
            return;

        const int32 OldCount = OldValues.Num();
        const int32 NewCount = NewValues.Num();
        const int32 MinCount = FMath::Min(OldCount, NewCount);

        for (int32 i = 0; i < MinCount; i++)
        {
            auto &OldValue = OldValues[i];
            auto &NewValue = NewValues[i];

            if (OldValue == NewValue)
                continue;

            auto ItemPath = BasePath;
            ItemPath.Segments.Emplace(TInPlaceType<FListIndexSegment>{}, i);

            CollectDiffs(OldValue, NewValue, Edits, ItemPath);
        }

        if (NewCount > OldCount)
        {
            for (int32 i = MinCount; i < NewCount; i++)
            {
                if (i == NewCount - 1)
                {
                    Edits.Emplace(TInPlaceType<FListAddEdit>{}, BasePath, SerializeToJson(NewValues[i]));
                }
                else
                {
                    Edits.Emplace(TInPlaceType<FListInsertEdit>{}, BasePath, i, SerializeToJson(NewValues[i]));
                }
            }
        }
        else if (NewCount < OldCount)
        {
            for (int32 i = OldCount - 1; i >= NewCount; i--)
            {
                auto &RemoveEdit =
                    Edits.Emplace_GetRef(TInPlaceType<FListRemoveAtEdit>{}, BasePath, i).Get<FListRemoveAtEdit>();
                RemoveEdit.OriginalItem = SerializeToJson(OldValues[i]);
            }
        }
    }

    template <typename T>
    concept TCanApplyFieldEdit =
        requires(T &Value, const FFieldEdit &Edit, const TConstArrayView<FFieldPathSegment> Path) {
            { ApplyFieldEdit(Value, Edit, Path) } -> std::convertible_to<std::expected<void, FString>>;
        };

    template <typename T>
        requires TJsonObject<T> || (THasEqualityCheck<T> && TJsonDeserializable<T>)
    std::expected<void, FString> ApplyFieldEdit(T &Value,
                                                const FFieldEdit &Edit,
                                                const TConstArrayView<FFieldPathSegment> Path)
    {
        if constexpr (TJsonObject<T>)
        {
            if (Path.Num() != 0)
            {
                return Visit(UE::Overload(
                                 [&](const FPropertySegment &Segment) -> std::expected<void, FString>
                                 {
                                     auto SubPath = Path.RightChop(1);
                                     std::expected<void, FString> Result =
                                         std::unexpected(TEXT("Failed to traverse into object"));
                                     TJsonObjectTraits<T>::JsonSchema.ForEachFieldWithBreak(
                                         [&]<auto Member>(const TJsonField<Member> &Field)
                                         {
                                             if (Field.CppName != Segment.Name)
                                                 return true;

                                             auto &CurrentValue = Field.GetMember(Value);
                                             Result = ApplyFieldEdit(CurrentValue, Edit, SubPath);
                                             return false;
                                         });

                                     return Result;
                                 },
                                 [](auto &&) -> std::expected<void, FString>
                                 { return std::unexpected(TEXT("Cannot traverse into scalar value")); }),
                             Path[0]);
            }
        }
        else
        {
            if (Path.Num() != 0)
            {
                return std::unexpected(
                    FString::Printf(TEXT("Cannot traverse into scalar value, path still has %d segment(s)."),
                                    Path.Num()));
            }
        }

        return Visit(UE::Overload(
                         [&Value](const FSetValueEdit &Set) -> std::expected<void, FString>
                         {
                             return DeserializeFromJson<T>(Set.NewValue)
                                 .transform([&Value](T &&DeserializedValue) { Value = MoveTemp(DeserializedValue); });
                         },
                         [](auto &&) -> std::expected<void, FString>
                         { return std::unexpected(TEXT("Scalar can only take FSetValueEdit")); }),
                     Edit);
    }

    template <TCanApplyFieldEdit T>
    std::expected<void, FString> ApplyFieldEdit(TArray<T> &Value,
                                                const FFieldEdit &Edit,
                                                const TConstArrayView<FFieldPathSegment> Path)
    {
        if (Path.Num() == 0)
        {
            return Visit(
                UE::Overload(
                    [&Value](const FListAddEdit &Add) -> std::expected<void, FString>
                    {
                        return DeserializeFromJson<T>(Add.NewItem)
                            .transform([&Value](T &&DeserializedValue) { Value.Emplace(MoveTemp(DeserializedValue)); });
                    },
                    [&Value](const FListInsertEdit &Insert) -> std::expected<void, FString>
                    {
                        return DeserializeFromJson<T>(Insert.NewItem)
                            .transform([&](T &&DeserializedValue)
                                       { Value.Insert(MoveTemp(DeserializedValue), Insert.Index); });
                    },
                    [&Value](const FListRemoveAtEdit &Remove) -> std::expected<void, FString>
                    {
                        Value.RemoveAt(Remove.Index, 1);
                        return {};
                    },
                    [&Value](const FListSwapEdit &Insert) -> std::expected<void, FString>
                    {
                        Value.Swap(Insert.IndexA, Insert.IndexB);
                        return {};
                    },
                    [](auto &&) -> std::expected<void, FString>
                    {
                        return std::unexpected(TEXT(
                            "List can only take FListAddEdit, FListInsertEdit, FListRemoveAtEdit or FListSwapEdit"));
                    }),
                Edit);
        }

        return Visit(UE::Overload(
                         [&](const FListIndexSegment &Index) -> std::expected<void, FString>
                         {
                             auto RemainingPath = Path.RightChop(1);

                             if (!Value.IsValidIndex(Index.Index))
                             {
                                 return std::unexpected(FString::Printf(TEXT("Index %d out of bounds"), Index.Index));
                             }

                             return ApplyFieldEdit(Value[Index.Index], Edit, RemainingPath);
                         },
                         [](auto &&) -> std::expected<void, FString>
                         { return std::unexpected(TEXT("List index can only take FListIndexSegment")); }),
                     Path[0]);
    }

    template <TCanApplyFieldEdit T>
    std::expected<void, FString> ApplyFieldEdit(T &Value, const FFieldEdit &Edit)
    {
        return ApplyFieldEdit(Value, Edit, GetPath(Edit).Segments);
    }
} // namespace PokeEdit
