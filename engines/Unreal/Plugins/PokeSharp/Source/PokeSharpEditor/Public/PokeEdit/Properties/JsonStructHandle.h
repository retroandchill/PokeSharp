// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "JsonPropertyHandle.h"
#include "LogPokeSharpEditor.h"
#include "Misc/NotifyHook.h"
#include "PokeEdit/PokeEditApi.h"
#include "PokeEdit/Schema/FieldPath.h"
#include "PokeEdit/Serialization/JsonSchema.h"

namespace PokeEdit
{
    class FJsonStructHandle : public FNotifyHook
    {
      public:
        explicit FJsonStructHandle(const UScriptStruct *ScriptStruct, FFieldPath BasePath)
            : Struct(ScriptStruct), BasePath(MoveTemp(BasePath))
        {
        }
        virtual ~FJsonStructHandle() = default;

        const UScriptStruct *GetStruct() const
        {
            return Struct;
        }

        const FFieldPath &GetBasePath() const
        {
            return BasePath;
        }

        void SetBasePath(FFieldPath Path)
        {
            BasePath = MoveTemp(Path);
        }

        virtual std::expected<TSharedRef<FStructOnScope>, FString> DeserializeFromJson(
            const TSharedRef<FJsonValue> &JsonValue) = 0;

      private:
        TObjectPtr<const UScriptStruct> Struct;
        FFieldPath BasePath;
    };

    template <TSerializableStruct T>
        requires TCanApplyFieldEdit<T> && TDiffableType<T>
    class TJsonStructHandle final : public FJsonStructHandle
    {
        constexpr static auto JsonSchema = TJsonObjectTraits<T>::JsonSchema;

      public:
        explicit TJsonStructHandle(FFieldPath BasePath) : FJsonStructHandle(GetScriptStruct<T>(), MoveTemp(BasePath))
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

            auto PropertyPath = GetBasePath();
            PropertyPath.Segments.Emplace(TInPlaceType<FPropertySegment>{}, PropertyThatChanged->GetFName());
            auto Diffs = (*ChangedProperty)->CollectDiffs(CurrentValue, PropertyPath);

            // For now, it's probably safe to assume that we'll only ever have one thing that changes, if that changes,
            // we'll have to rethink how the API is designed.
            check(Diffs.Num() <= 1);

            if (Diffs.Num() == 0)
            {
                (*ChangedProperty)->ClearCache();
                return;
            }

            if (auto RollbackResult = (*ChangedProperty)->Rollback(CurrentValue); !RollbackResult.has_value())
            {
                UE_LOG(LogPokeSharpEditor, Error, TEXT("%s"), *RollbackResult.error());
                return;
            }

            auto FinalResult = PerformFieldEdit(Diffs[0]).and_then(
                [&](const TArray<FFieldEdit> &Edits) -> std::expected<void, FString>
                {
                    for (auto &Edit : Edits)
                    {
                        TConstArrayView<FFieldPathSegment> Path = GetPath(Edit).Segments;
                        auto RelativePath = Path.RightChop(GetBasePath().Segments.Num());
                        if (auto EditResult = ApplyFieldEdit(CurrentValue, Edit, RelativePath); !EditResult.has_value())
                        {
                            return EditResult;
                        }
                    }

                    return {};
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
