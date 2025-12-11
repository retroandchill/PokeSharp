// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "FieldEditBuilder.h"
#include "PokeEdit/Schema/FieldEdit.h"
#include "PokeEdit/Serialization/JsonSchema.h"
#include "Structs/UnrealStruct.h"

namespace PokeEdit
{
    template <typename T>
    concept TSerializableStruct = UEStruct<T> && TJsonDeserializable<T> && TJsonSerializable<T> && TJsonObject<T>;

    template <TSerializableStruct T>
    class TJsonPropertyHandle
    {
      public:
        virtual ~TJsonPropertyHandle() = default;

        virtual void CacheCurrentValue(const T &Owner) = 0;

        virtual TArray<FFieldEdit> CollectDiffs(const T &Owner, const FFieldPath &BasePath) const = 0;

        virtual std::expected<void, FString> Rollback(T &Owner) = 0;

        virtual void ClearCache() = 0;
    };

    template <auto Member>
        requires TJsonFieldMember<Member> && TSerializableStruct<typename TMemberInfo<Member>::OwnerType>
    class TJsonPropertyHandleImpl final : public TJsonPropertyHandle<typename TJsonField<Member>::OwnerType>
    {
      public:
        using OwnerType = TJsonField<Member>::OwnerType;
        using MemberType = TJsonField<Member>::MemberType;

        void CacheCurrentValue(const OwnerType &Owner) override
        {
            Field.Emplace(Owner.*Member);
        }

        TArray<FFieldEdit> CollectDiffs(const OwnerType &Owner, const FFieldPath &BasePath) const override
        {
            if (!Field.IsSet())
                return {};

            TArray<FFieldEdit> Edits;
            PokeEdit::CollectDiffs(*Field, Owner.*Member, Edits, BasePath);
            return Edits;
        }

        std::expected<void, FString> Rollback(OwnerType &Owner) override
        {
            if (!Field.IsSet())
                return std::unexpected(TEXT("No cached value to rollback"));

            Owner.*Member = MoveTemp(Field.GetValue());
            Field.Reset();
            return {};
        }

        void ClearCache() override
        {
            Field.Reset();
        }

      private:
        TOptional<MemberType> Field;
    };
} // namespace PokeEdit
