// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "DiffNodeOperations.h"
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

        virtual TOptional<FDiffNode> CollectDiffs(const T &Owner, int32 Index) const = 0;

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

        TOptional<FDiffNode> CollectDiffs(const OwnerType &Owner, int32 Index) const override
        {
            if (!Field.IsSet())
                return NullOpt;

            return PokeEdit::Diff(Field.GetValue(), Owner.*Member);
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
