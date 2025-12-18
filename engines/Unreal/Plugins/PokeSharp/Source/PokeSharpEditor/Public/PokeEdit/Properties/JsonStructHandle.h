// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Misc/NotifyHook.h"
#include "UObject/StructOnScope.h"

namespace PokeEdit
{
    class FJsonStructHandle : public FNotifyHook
    {
      public:
        explicit FJsonStructHandle(const UScriptStruct *ScriptStruct, const FName TabName, const int32 Index)
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

    template <typename T>
    concept TCanCreateJsonStructHandle = requires(const FName Name, const int32 Index) {
        { T::CreateJsonHandle(Name, Index) } -> std::convertible_to<TSharedRef<FJsonStructHandle>>;
    };
} // namespace PokeEdit
