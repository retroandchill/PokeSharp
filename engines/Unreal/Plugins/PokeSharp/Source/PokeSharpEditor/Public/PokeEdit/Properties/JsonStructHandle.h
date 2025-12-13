// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "JsonPropertyHandle.h"
#include "LogPokeSharpEditor.h"
#include "Misc/NotifyHook.h"
#include "PokeEdit/PokeEditApi.h"
#include "PokeEdit/Serialization/JsonSchema.h"

namespace PokeEdit
{
    class FJsonStructHandle : public FNotifyHook
    {
      public:
        explicit FJsonStructHandle(const UScriptStruct *ScriptStruct, const FName TabName,  const int32 Index)
            : Struct(ScriptStruct), TabName(TabName), Index(Index)
        {
        }
        virtual ~FJsonStructHandle() = default;

        const UScriptStruct *GetStruct() const
        {
            return Struct;
        }
        
        FName GetTabName() const
        {
            return TabName;
        }

        int32 GetIndex() const
        {
            return Index;
        }

        void SetIndex(const int32 InIndex)
        {
            Index = InIndex;
        }

        virtual std::expected<TSharedRef<FStructOnScope>, FString> DeserializeFromJson(
            const TSharedRef<FJsonValue> &JsonValue) = 0;

      private:
        TObjectPtr<const UScriptStruct> Struct;
        FName TabName;
        int32 Index;
    };

    template <TSerializableStruct T>
        requires TCanApplyDiff<T> && TDiffableType<T>
    class TJsonStructHandle final : public FJsonStructHandle
    {
        constexpr static auto JsonSchema = TJsonObjectTraits<T>::JsonSchema;

      public:
        explicit TJsonStructHandle(const FName TabName, const int32 Index) : FJsonStructHandle(GetScriptStruct<T>(), TabName, Index)
        {
        }

        std::expected<TSharedRef<FStructOnScope>, FString> DeserializeFromJson(
            const TSharedRef<FJsonValue> &JsonValue) override
        {
            return PokeEdit::DeserializeFromJson<T>(JsonValue).transform(
                [this](T &&Value)
                {
                    CurrentValue = MoveTemp(Value);
                    return MakeShared<FStructOnScope>(GetStruct(), std::bit_cast<uint8 *>(&CurrentValue));
                });
        }

        void NotifyPreChange(FProperty *PropertyAboutToChange) override
        {
            const auto *PropertyToChange = Properties.Find(PropertyAboutToChange->GetFName());
            if (PropertyToChange == nullptr)
            {
                return;
            }

            (*PropertyToChange)->CacheCurrentValue(CurrentValue);
        }

        void NotifyPostChange(const FPropertyChangedEvent &PropertyChangedEvent,
                              FProperty *PropertyThatChanged) override
        {
            const auto *ChangedProperty = Properties.Find(PropertyThatChanged->GetFName());
            if (ChangedProperty == nullptr)
            {
                return;
            }
            
            auto Diffs = (*ChangedProperty)->CollectDiffs(CurrentValue, GetIndex());
            if (!Diffs.IsSet())
            {
                (*ChangedProperty)->ClearCache();
                return;
            }
            
            if (auto RollbackResult = (*ChangedProperty)->Rollback(CurrentValue); !RollbackResult.has_value())
            {
                UE_LOG(LogPokeSharpEditor, Error, TEXT("%s"), *RollbackResult.error());
                return;
            }
            
            TMap<FName, TSharedRef<FDiffNode>> DiffsMap;
            DiffsMap.Add(PropertyThatChanged->GetFName(), MakeShared<FDiffNode>(MoveTemp(*Diffs)));

            auto FinalResult = UpdateEntityAtIndex(GetTabName(), GetIndex(), FObjectDiffNode(MoveTemp(DiffsMap)))
                .and_then([&](const FEntityUpdateResponse &Edits) -> std::expected<void, FString>
                    {
                        if (!Edits.Diff.IsSet())
                        {
                            return {};
                        }
                        
                        return ApplyEdit(CurrentValue, *Edits.Diff);
                    });

            if (!FinalResult.has_value())
            {
                UE_LOG(LogPokeSharpEditor, Error, TEXT("%s"), *FinalResult.error());
            }
        }

      private:
        static TMap<FName, TSharedRef<TJsonPropertyHandle<T>>> CreateProperties()
        {
            const UScriptStruct *StaticStruct = GetScriptStruct<T>();

            TMap<FName, TSharedRef<TJsonPropertyHandle<T>>> Handles;

            JsonSchema.ForEachField(
                [&]<auto Member>(const TJsonField<Member> &Field)
                {
                    const FProperty *Property = StaticStruct->FindPropertyByName(FName(Field.CppName));
                    if (Property == nullptr)
                        return;

                    Handles.Add(Property->GetFName(), MakeShared<TJsonPropertyHandleImpl<Member>>());
                });

            return Handles;
        }

        T CurrentValue;
        TMap<FName, TSharedRef<TJsonPropertyHandle<T>>> Properties = CreateProperties();
    };
} // namespace PokeEdit
