// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Dom/JsonValue.h"
#include "JsonObjectConverter.h"
#include "Templates/ValueOrError.h"
#include "UObject/StructOnScope.h"
#include <expected>

namespace PokeEdit
{
    using FCustomImportCallback = FJsonObjectConverter::CustomImportCallback;

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
        { TJsonConverter<std::remove_cvref_t<T>>::Serialize(Value) } -> std::convertible_to<TSharedRef<FJsonValue>>;
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

    /**
     * Determines if a type is an Enum that can be converted to a string value.
     *
     * @tparam T The source enum type
     */
    template <typename T>
    concept TPrintableEnum = std::is_enum_v<T> && requires(T Enum) {
        { LexToString(Enum) } -> std::same_as<FString>;
    };

    /**
     * Determines if a type is an Enum that can be parsed from a string value,
     * represented by a null-terminated character array.
     *
     * @tparam T The destination type
     */
    template <typename T>
    concept TParsableEnumFromLiteral = std::is_enum_v<T> && requires(const TCHAR *Lex, T &OutEnum) {
        { LexFromString(OutEnum, Lex) } -> std::same_as<bool>;
    };

    /**
     * Determines if a type is an Enum that can be parsed from a string value,
     * represented by string-view object
     *
     * @tparam T The destination type
     */
    template <typename T>
    concept TParsableEnumFromStringView = std::is_enum_v<T> && requires(const FStringView Lex, T &OutEnum) {
        { LexFromString(OutEnum, Lex) } -> std::same_as<bool>;
    };

    /**
     * Determines if a type is an Enum that can be parsed from a string value, represented by string object.
     *
     * @tparam T The destination type
     */
    template <typename T>
    concept TParsableEnumFromString = std::is_enum_v<T> && requires(const FString &Lex, T &OutEnum) {
        { LexFromString(OutEnum, Lex) } -> std::same_as<bool>;
    };

    /**
     * Determines if a type is an Enum that can be parsed from a string value.
     *
     * @tparam T The destination type
     */
    template <typename T>
    concept TParsableEnum = TParsableEnumFromLiteral<T> || TParsableEnumFromStringView<T> || TParsableEnumFromString<T>;

    /**
     * Attempt to parse a string object into an nnum value.
     *
     * @tparam T The destination type
     * @param Lex The source string object
     * @return The enum value if a matching literal was found, otherwise an empty optional.
     */
    template <TParsableEnum T>
    constexpr TOptional<T> ParseEnum(const FString &Lex)
    {
        T Result;
        if constexpr (TParsableEnumFromString<T>)
        {
            return LexFromString(Result, Lex) ? TOptional<T>(Result) : TOptional<T>();
        }
        else if constexpr (TParsableEnumFromStringView<T>)
        {
            return LexFromString(Result, Lex) ? TOptional<T>(Result) : TOptional<T>();
        }
        else
        {
            return LexFromString(Result, Lex.GetCharArray().GetData()) ? TOptional<T>(Result) : TOptional<T>();
        }
    }

    /**
     * Converter for handling converting enums to and from JSON.
     *
     * @tparam T The target enum type
     */
    template <typename T>
        requires TPrintableEnum<T> || TParsableEnum<T>
    struct TJsonConverter<T>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<T, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
            requires TParsableEnum<T>
        {
            if (FString Result; Value->TryGetString(Result))
            {
                if (T Enum; LexFromString(Enum, Result))
                {
                    return MakeValue(Enum);
                }

                return MakeError(FString::Format(TEXT("Value '{0}' is not a valid enum value"), {Result}));
            }

            return MakeError(FString::Format(TEXT("Value '{0}' is not a string"), {WriteAsString(Value)}));
        }

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const T Value)
            requires TPrintableEnum<T>
        {
            return MakeShared<FJsonValueString>(LexToString(Value));
        }
    };

    /**
     * Converter for the JSON representation of list/array values.
     *
     * @tparam T The inner type of the array.
     */
    template <typename T>
        requires TJsonDeserializable<T> || TJsonSerializable<T>
    struct TJsonConverter<TArray<T>>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<TArray<T>, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
            requires TJsonDeserializable<T>
        {
            if (const TArray<TSharedPtr<FJsonValue>> *JsonValues; Value->TryGetArray(JsonValues))
            {
                TArray<FString> Errors;
                TArray<T> Result;
                Result.Reserve(JsonValues->Num());
                for (const auto &JsonValue : *JsonValues)
                {
                    if (auto DeserializedValue = TJsonConverter<T>::Deserialize(JsonValue.ToSharedRef());
                        DeserializedValue.has_value())
                    {
                        Result.Add(MoveTemp(DeserializedValue).value());
                    }
                    else
                    {
                        Errors.Add(MoveTemp(DeserializedValue).error());
                    }
                    if (Errors.Num() > 0)
                    {
                        return std::unexpected(
                            FString::Format(TEXT("Multiple errors were found when deserializing value\n {0}"),
                                            {FString::Join(Errors, TEXT("\n"))}));
                    }
                }

                return MoveTemp(Result);
            }

            return std::unexpected(FString::Format(TEXT("Value '{0}' is not an array"), {WriteAsString(Value)}));
        }

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const TArray<T> &Value)
            requires TJsonSerializable<T>
        {
            TArray<TSharedPtr<FJsonValue>> JsonValues;
            JsonValues.Reserve(Value.Num());
            for (const auto &Item : Value)
            {
                JsonValues.Add(TJsonConverter<T>::Serialize(Item));
            }

            return MakeShared<FJsonValueArray>(MoveTemp(JsonValues));
        }
    };

    /**
     * Converter for the JSON representation of optional JSON values.
     *
     * @tparam T The inner type of the array.
     */
    template <typename T>
        requires TJsonDeserializable<T> || TJsonSerializable<T>
    struct TJsonConverter<TOptional<T>>
    {
        /**
         * Attempts to deserialize a JSON value to the target type.
         *
         * @param Value The input JSON value
         * @return Either the deserialized value, or an error message explaining why serialization failed.
         */
        static std::expected<TOptional<T>, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
            requires TJsonDeserializable<T>
        {
            if (Value->IsNull())
            {
                return TOptional<T>(NullOpt);
            }

            return TJsonConverter<T>::Deserialize(Value).transform([](T &&Result)
                                                                   { return TOptional<T>(MoveTemp(Result)); });
        }

        /**
         * Serializes a value to the target type.
         *
         * @param Value The input value
         * @return The serialized JSON value
         */
        static TSharedRef<FJsonValue> Serialize(const TOptional<T> &Value)
            requires TJsonSerializable<T>
        {
            return Value.IsSet() ? TJsonConverter<T>::Serialize(Value.GetValue()) : MakeShared<FJsonValueNull>();
        }
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
        static TSharedPtr<FJsonValue> Serialize(const TSharedPtr<T> &Value)
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

    POKESHARPEDITOR_API std::expected<TSharedRef<FStructOnScope>, FText> DeserializeFromJson(
        const TSharedRef<FJsonValue> &Value,
        const UStruct *Struct,
        int64 CheckFlags = 0,
        int64 SkipFlags = 0,
        const bool bStrictMode = false,
        const FCustomImportCallback *ImportCb = nullptr);

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