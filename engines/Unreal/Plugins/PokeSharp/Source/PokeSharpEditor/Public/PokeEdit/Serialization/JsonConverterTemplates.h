// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "JsonConverter.h"

namespace PokeEdit
{
    /**
     * Determines if a type is an Enum that can be converted to a string value.
     *
     * @tparam T The source enum type
     */
    template <typename T>
    concept TPrintableEnum = std::is_enum_v<T> && requires(T Enum) {
        { LexToString(Enum) } -> std::convertible_to<FString>;
    };

    template <typename T>
    constexpr FString PrintEnum(const T Value)
    {
        return LexToString(Value);
    }

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
    constexpr std::expected<T, FString> ParseEnum(const FString &Lex)
    {
        T Result;
        if constexpr (TParsableEnumFromString<T>)
        {
            return LexFromString(Result, Lex)
                       ? std::expected<T, FString>(Result)
                       : std::expected<T, FString>(
                             std::unexpect,
                             FString::Format(TEXT("Value '{0}' is not a valid enum value"), {Lex}));
        }
        else if constexpr (TParsableEnumFromStringView<T>)
        {
            return LexFromString(Result, Lex)
                       ? std::expected<T, FString>(Result)
                       : std::expected<T, FString>(
                             std::unexpect,
                             FString::Format(TEXT("Value '{0}' is not a valid enum value"), {Lex}));
        }
        else
        {
            return LexFromString(Result, Lex.GetCharArray().GetData())
                       ? std::expected<T, FString>(Result)
                       : std::expected<T, FString>(
                             std::unexpect,
                             FString::Format(TEXT("Value '{0}' is not a valid enum value"), {Lex}));
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
                return ParseEnum<T>(Result);
            }

            return std::unexpected(FString::Format(TEXT("Value '{0}' is not a string"), {WriteAsString(Value)}));
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
            return MakeShared<FJsonValueString>(PrintEnum(Value));
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

    template <typename>
    struct TMapKeyTraits;

    template <>
    struct TMapKeyTraits<FName>
    {
        static std::expected<FName, FString> Deserialize(FStringView String)
        {
            return FName(String);
        }

        static FString Serialize(const FName Key)
        {
            return Key.ToString();
        }
    };

    template <>
    struct TMapKeyTraits<FString>
    {
        static std::expected<FString, FString> Deserialize(const FString &String)
        {
            return String;
        }

        static std::expected<FString, FString> Deserialize(FString &&String)
        {
            return MoveTemp(String);
        }

        static FString Serialize(const FString &Key)
        {
            return Key;
        }

        static FString Serialize(FString &&Key)
        {
            return MoveTemp(Key);
        }
    };

    template <typename T>
        requires TPrintableEnum<T> || TParsableEnum<T>
    struct TMapKeyTraits<T>
    {
        static std::expected<T, FString> Deserialize(const FString &String)
        {
            return ParseEnum<T>(String);
        }

        static FString Serialize(const T Key)
        {
            return PrintEnum(Key);
        }
    };

    template <typename T>
    concept TParsableMapKey = requires(const FString &String) {
        { TMapKeyTraits<T>::Deserialize(String) } -> std::convertible_to<std::expected<T, FString>>;
    };

    template <typename T>
    concept TPrintableMapKey = requires(const T &Key) {
        { TMapKeyTraits<T>::Serialize(Key) } -> std::convertible_to<FString>;
    };

    template <typename K, typename V>
        requires((TParsableMapKey<K> || TPrintableMapKey<K>) && (TJsonDeserializable<V> || TJsonSerializable<V>))
    struct TJsonConverter<TMap<K, V>>
    {
        static std::expected<TMap<K, V>, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
            requires TParsableMapKey<K> && TJsonDeserializable<V>
        {
            const auto Object = Value->AsObject();
            if (Object == nullptr)
            {
                return std::unexpected(FString::Format(TEXT("Value '{0}' is not an object"), {WriteAsString(Value)}));
            }

            TMap<FName, V> Result;
            for (const auto &[Key, JsonValue] : Object->Values)
            {
                auto KeyValue = TMapKeyTraits<K>::Deserialize(Key);
                if (!KeyValue.has_value())
                {
                    return std::unexpected(MoveTemp(KeyValue).error());
                }

                auto DeserializedValue = TJsonConverter<V>::Deserialize(JsonValue.ToSharedRef());
                if (!DeserializedValue.has_value())
                {
                    return std::unexpected(MoveTemp(DeserializedValue).error());
                }

                Result.Add(MoveTemp(KeyValue).value(), MoveTemp(DeserializedValue).value());
            }

            return MoveTemp(Result);
        }

        static TSharedRef<FJsonValue> Serialize(const TMap<K, V> &Value)
            requires TPrintableMapKey<K> && TJsonSerializable<V>
        {
            auto JsonObject = MakeShared<FJsonObject>();
            for (const auto &[Key, MapValue] : Value)
            {
                JsonObject->SetField(TMapKeyTraits<K>::Serialize(Key), TJsonConverter<V>::Serialize(MapValue));
            }
            return MakeShared<FJsonValueObject>(JsonObject);
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
} // namespace PokeEdit