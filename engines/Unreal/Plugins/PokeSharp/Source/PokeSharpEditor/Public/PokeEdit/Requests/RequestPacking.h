// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "RequestPayload.h"
#include "PokeEdit/Serialization/JsonConverter.h"

namespace PokeEdit
{
    template <typename>
    struct TIsDirectlyPackable : std::false_type { };
    
    template <>
    struct TIsDirectlyPackable<bool> : std::true_type { };
    
    template <>
    struct TIsDirectlyPackable<uint8> : std::true_type { };
    
    template <>
    struct TIsDirectlyPackable<int32> : std::true_type { };
    
    template <>
    struct TIsDirectlyPackable<int64> : std::true_type { };
    
    template <>
    struct TIsDirectlyPackable<FName> : std::true_type { };
    
    template <>
    struct TIsDirectlyPackable<FGuid> : std::true_type { };
    
    template <>
    struct TIsDirectlyPackable<FString> : std::true_type { };
    
    template <>
    struct TIsDirectlyPackable<TArray<uint8>> : std::true_type { };
    
    template <typename T>
        requires std::is_enum_v<T>
    struct TIsDirectlyPackable<T> : std::true_type { };
    
    template <typename T>
    concept TDirectlyPackable = TIsDirectlyPackable<std::remove_cvref_t<T>>::value;
    
    template <typename T>
    concept TPackable = TDirectlyPackable<T> || TJsonSerializable<T>;
    
    template <TPackable T>
    using TPackedType = std::conditional_t<TDirectlyPackable<T>, std::remove_cvref_t<T>, TArray<uint8>>;
    
    POKESHARPEDITOR_API std::expected<TArray<uint8>, FString> WriteJsonToBuffer(const TSharedRef<FJsonValue>& JsonValue);
    
    template <TPackable T>
    constexpr std::expected<TPackedType<T>, FString> PackValue(T&& Value)
    {
        if constexpr (TDirectlyPackable<T>)
        {
            return Forward<T>(Value);
        }
        else
        {
            static_assert(TJsonSerializable<T>, "Value is not directly packable and does not implement the JsonSerializable interface");
            auto AsJson = SerializeToJson(Forward<T>(Value));
            return WriteJsonToBuffer(AsJson);
        }
    }

    namespace Private
    {
        template <typename>
        struct TIsStdExpected : std::false_type { };

        template <typename V, typename E>
        struct TIsStdExpected<std::expected<V, E>> : std::true_type { };

        template <typename T>
        inline constexpr bool TIsStdExpected_V = TIsStdExpected<std::remove_cvref_t<T>>::value;

        template <typename Tuple, SIZE_T... I>
        bool TryGetFirstError(const Tuple& PackedValues, FString& OutError, std::index_sequence<I...>)
        {
            bool bFound = false;

            // Left-to-right short-circuit (first error wins).
            (void)((!bFound && !std::get<I>(PackedValues).has_value()
                        ? (OutError = std::get<I>(PackedValues).error(), bFound = true, true)
                        : false) || ...);

            return bFound;
        }
    }
    
    template <TPackable... Args>
        requires (sizeof...(Args) <= 8)
    constexpr std::expected<TRequestPayload<TPackedType<Args>...>, FString> PackPayload(Args&&... InArgs)
    {
        auto PackedValues = std::make_tuple(PackValue(Forward<Args>(InArgs))...);

        if (FString Error; Private::TryGetFirstError(PackedValues, Error, std::make_index_sequence<sizeof...(Args)>{}))
        {
            return std::unexpected(MoveTemp(Error));
        }

        return std::apply(
            [](auto&&... Packed) -> std::expected<TRequestPayload<TPackedType<Args>...>, FString>
            {
                // Safe: we only get here if all .has_value() are true.
                return TRequestPayload<TPackedType<Args>...>{ MoveTemp(Packed).value()... };
            },
            MoveTemp(PackedValues));
    }
}
