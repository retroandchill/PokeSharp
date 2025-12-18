// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Containers/Array.h"
#include "Dom/JsonValue.h"
#include "JsonObjectConverter.h"
#include "Templates/ValueOrError.h"
#include "Types/AttributeStorage.h"
#include <expected>

namespace PokeEdit
{
    /**
     * Convert a JSON value to its serialized String representation. Used primarily for printing the value on in
     * debug/error logs.
     *
     * @param Value The JSON value to convert.
     * @return The serialized string.
     */
    POKESHARPEDITOR_API FString WriteAsString(const TSharedRef<FJsonValue> &Value);

    /**
     * Template meta-type used to define if a type can be converted either to or from JSON.<br>
     * To define a custom converter, create a template specialization for the target type and implement the
     * following two methods:
     * - static std::expected<T, FString> Deserialize(const TSharedRef<FJsonValue>& Value);
     * - static TSharedRef<FJsonValue> Deserialize(const T& Value);
     *
     * @tparam T The type convert.
     */
    template <typename T>
    struct TJsonConverter;

    /**
     * Defines if a type can be deserialized from a JSON value.
     *
     * @tparam T The output type
     */
    template <typename T>
    concept TJsonDeserializable = requires(const TSharedRef<FJsonValue> &Value) {
        {
            TJsonConverter<std::remove_cvref_t<T>>::Deserialize(Value)
        } -> std::convertible_to<std::expected<T, FString>>;
    };

    /**
     * Defines if a type can serialized to JSON.
     *
     * @tparam T The input type
     */
    template <typename T>
    concept TJsonSerializable = requires(const T &Value) {
        {
            TJsonConverter<std::remove_cvref_t<T>>::Serialize(Value)
        } -> std::convertible_to<TSharedRef<const FJsonValue>>;
    };

    /**
     * Defines if a type can be converted both to and from JSON.
     *
     * @tparam T The target type
     */
    template <typename T>
    concept TJsonConvertible = TJsonDeserializable<T> && TJsonSerializable<T>;

    /**
     * Converter for JSON representation of boolean values.
     */
    template <>
    struct POKESHARPEDITOR_API TJsonConverter<bool>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<bool, FString> Deserialize(const TSharedRef<FJsonValue> &Value);

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const bool Value)
        {
            return MakeShared<FJsonValueBoolean>(Value);
        }
    };

    /**
     * Converter for handling numeric values.
     *
     * @tparam T The type of value, must represent either an integer or floating point number.
     */
    template <typename T>
        requires std::is_integral_v<T> || std::is_floating_point_v<T>
    struct TJsonConverter<T>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<T, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            if (T Result; Value->TryGetNumber(Result))
            {
                return Result;
            }

            return std::unexpected(FString::Format(TEXT("Value '{0}' is not a number"), {WriteAsString(Value)}));
        }

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const T Value)
        {
            return MakeShared<FJsonValueNumber>(Value);
        }
    };

    /**
     * Converter for handling the JSON serialization of Name values.
     */
    template <>
    struct POKESHARPEDITOR_API TJsonConverter<FName>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<FName, FString> Deserialize(const TSharedRef<FJsonValue> &Value);

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const FName Value)
        {
            return MakeShared<FJsonValueString>(Value.ToString());
        }
    };

    /**
     * Converter for handling the JSON serialization of String values.
     */
    template <>
    struct POKESHARPEDITOR_API TJsonConverter<FString>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<FString, FString> Deserialize(const TSharedRef<FJsonValue> &Value);

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const FString &Value)
        {
            return MakeShared<FJsonValueString>(Value);
        }

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(FString &&Value)
        {
            return MakeShared<FJsonValueString>(MoveTemp(Value));
        }

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(FStringView Value)
        {
            return MakeShared<FJsonValueString>(FString(Value));
        }
    };

    /**
     * Converter for handling the JSON serialization of Text values.
     *
     * @note This type will serialize/deserialize from the full localized string (i.e. NSLOCTEXT) form if possible
     */
    template <>
    struct POKESHARPEDITOR_API TJsonConverter<FText>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<FText, FString> Deserialize(const TSharedRef<FJsonValue> &Value);

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const FText &Value);
    };

    template <>
    struct TJsonConverter<TSharedRef<FJsonValue>>
    {
        static std::expected<TSharedRef<FJsonValue>, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            return Value;
        }

        static const TSharedRef<FJsonValue> &Serialize(const TSharedRef<FJsonValue> &Value)
        {
            return Value;
        }
    };

    template <typename T>
        requires TJsonDeserializable<TSharedRef<T>> || TJsonSerializable<TSharedRef<T>>
    struct TJsonConverter<TSharedPtr<T>>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<TSharedPtr<T>, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
            requires TJsonDeserializable<TSharedRef<T>>
        {
            if (Value->IsNull())
            {
                return TSharedPtr<T>(nullptr);
            }

            return TJsonConverter<TSharedRef<T>>::Deserialize(Value).transform([](const TSharedRef<T> &Result)
                                                                               { return Result.ToSharedPtr(); });
        }

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const TSharedPtr<T> &Value)
            requires TJsonSerializable<TSharedRef<T>>
        {
            return Value != nullptr ? TJsonConverter<TSharedRef<T>>::Serialize(Value.ToSharedRef())
                                    : MakeShared<FJsonValueNull>();
        }
    };

    /**
     * Attempts to deserialize a JSON value to the target type.
     *
     * @tparam T The target type
     * @param Value The input JSON value
     * @return Either the deserialized value, or an error message explaining why serialization failed.
     */
    template <TJsonDeserializable T>
    std::expected<T, FString> DeserializeFromJson(const TSharedRef<FJsonValue> &Value)
    {
        return TJsonConverter<std::remove_cvref_t<T>>::Deserialize(Value);
    }

    /**
     * Serializes a value to the target type.
     *
     * @tparam T The target type to serialize to.
     * @param Value The input value
     * @return The serialized JSON value
     */
    template <TJsonSerializable T>
    TSharedRef<FJsonValue> SerializeToJson(T &&Value)
    {
        return TJsonConverter<std::remove_cvref_t<T>>::Serialize(Forward<T>(Value));
    }

} // namespace PokeEdit