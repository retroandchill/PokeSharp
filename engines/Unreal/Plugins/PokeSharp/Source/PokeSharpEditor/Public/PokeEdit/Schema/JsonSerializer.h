// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

namespace PokeEdit
{
    POKESHARPEDITOR_API FString WriteAsString(const TSharedRef<FJsonValue> &Value);

    template <typename T>
    struct TJsonConverter;

    template <typename T>
    concept TJsonDeserializable = requires(const TSharedRef<FJsonValue> &Value) {
        { TJsonConverter<T>::Deserialize(Value) } -> std::same_as<TValueOrError<T, FString>>;
    };

    template <typename T>
    concept TJsonSerializable = requires(const T &Value) {
        { TJsonConverter<T>::Serialize(Value) } -> std::same_as<TSharedRef<FJsonValue>>;
    };

    template <typename T>
    concept TJsonConvertible = TJsonDeserializable<T> && TJsonSerializable<T>;

    template <>
    struct TJsonConverter<bool>
    {
        static TValueOrError<bool, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            if (bool Result; Value->TryGetBool(Result))
            {
                return MakeValue(Result);
            }

            return MakeError(FString::Format(TEXT("Value '{0}' is not a boolean"), {WriteAsString(Value)}));
        }
        
        static TSharedRef<FJsonValue> Serialize(const bool Value)
        {
            return MakeShared<FJsonValueBoolean>(Value);
        }
    };

    template <typename T>
        requires std::is_integral_v<T> || std::is_floating_point_v<T>
    struct TJsonConverter<T>
    {
        static TValueOrError<T, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            if (T Result; Value->TryGetNumber(Result))
            {
                return MakeValue(Result);
            }

            return MakeError(FString::Format(TEXT("Value '{0}' is not a number"), {WriteAsString(Value)}));
        }

        static TSharedRef<FJsonValue> Serialize(const T Value)
        {
            return MakeShared<FJsonValueNumber>(Value);
        }
    };

    template <>
    struct TJsonConverter<FName>
    {
        static TValueOrError<FName, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            if (FString Result; Value->TryGetString(Result))
            {
                return MakeValue(FName(Result));
            }

            return MakeError(FString::Format(TEXT("Value '{0}' is not a string"), {WriteAsString(Value)}));
        }

        static TSharedRef<FJsonValue> Serialize(const FName Value)
        {
            return MakeShared<FJsonValueString>(Value.ToString());
        }
    };

    template <>
    struct TJsonConverter<FString>
    {
        static TValueOrError<FString, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            if (FString Result; Value->TryGetString(Result))
            {
                return MakeValue(Result);
            }

            return MakeError(FString::Format(TEXT("Value '{0}' is not a string"), {WriteAsString(Value)}));
        }

        static TSharedRef<FJsonValue> Serialize(const FString &Value)
        {
            return MakeShared<FJsonValueString>(Value);
        }

        static TSharedRef<FJsonValue> Serialize(FString &&Value)
        {
            return MakeShared<FJsonValueString>(MoveTemp(Value));
        }

        static TSharedRef<FJsonValue> Serialize(FStringView Value)
        {
            return MakeShared<FJsonValueString>(FString(Value));
        }
    };

    template <>
    struct TJsonConverter<FText>
    {
        static TValueOrError<FText, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
        {
            if (FString Result; Value->TryGetString(Result))
            {
                FText LocalizedText;
                if (!FTextStringHelper::ReadFromBuffer(Result.GetCharArray().GetData(), LocalizedText))
                {
                    LocalizedText = FText::FromString(Result);
                }
                return MakeValue(MoveTemp(LocalizedText));
            }

            return MakeError(FString::Format(TEXT("Value '{0}' is not a string"), {WriteAsString(Value)}));
        }

        static TSharedRef<FJsonValue> Serialize(const FText &Value)
        {
            FString Buffer;
            FTextStringHelper::WriteToBuffer(Buffer, Value);
            return MakeShared<FJsonValueString>(MoveTemp(Buffer));
        }
    };

    template <typename T>
    concept TPrintableEnum = std::is_enum_v<T> && requires(T Enum) {
        { LexToString(Enum) } -> std::same_as<FString>;
    };

    template <typename T>
    concept TParsableEnumFromLiteral = std::is_enum_v<T> && requires(const TCHAR *Lex, T &OutEnum) {
        { LexFromString(OutEnum, Lex) } -> std::same_as<bool>;
    };

    template <typename T>
    concept TParsableEnumFromStringView = std::is_enum_v<T> && requires(const FStringView Lex, T &OutEnum) {
        { LexFromString(OutEnum, Lex) } -> std::same_as<bool>;
    };

    template <typename T>
    concept TParsableEnumFromString = std::is_enum_v<T> && requires(const FString &Lex, T &OutEnum) {
        { LexFromString(OutEnum, Lex) } -> std::same_as<bool>;
    };

    template <typename T>
    concept TParsableEnum = TParsableEnumFromLiteral<T> || TParsableEnumFromStringView<T> || TParsableEnumFromString<T>;

    template <typename T>
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

    template <typename T>
        requires TPrintableEnum<T> || TParsableEnum<T>
    struct TJsonConverter<T>
    {
        static TValueOrError<T, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
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

        static TSharedRef<FJsonValue> Serialize(const T Value)
            requires TPrintableEnum<T>
        {
            return MakeShared<FJsonValueString>(LexToString(Value));
        }
    };

    template <typename T>
        requires TJsonDeserializable<T> || TJsonSerializable<T>
    struct TJsonConverter<TArray<T>>
    {
        static TValueOrError<TArray<T>, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
            requires TJsonDeserializable<T>
        {
            if (const TArray<TSharedPtr<FJsonValue>> *JsonValues; Value->TryGetArray(JsonValues))
            {
                TArray<FString> Errors;
                TArray<T> Result;
                Result.Reserve(JsonValues->Num());
                for (const auto &JsonValue : *JsonValues)
                {
                    auto DeserializedValue = TJsonConverter<T>::Deserialize(JsonValue.ToSharedRef());
                    if (auto *Error = DeserializedValue.TryGetError(); Error != nullptr)
                    {
                        Errors.Add(MoveTemp(*Error));
                    }
                    else
                    {
                        Result.Add(MoveTemp(DeserializedValue.GetValue()));
                    }
                    if (Errors.Num() > 0)
                    {
                        return MakeError(
                            FString::Format(TEXT("Multiple errors were found when deserializing value\n {0}"),
                                            {FString::Join(Errors, TEXT("\n"))}));
                    }
                }

                return MakeValue(MoveTemp(Result));
            }

            return MakeError(FString::Format(TEXT("Value '{0}' is not an array"), {WriteAsString(Value)}));
        }

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

    template <typename T>
        requires TJsonDeserializable<T> || TJsonSerializable<T>
    struct TJsonConverter<TOptional<T>>
    {
        static TValueOrError<TOptional<T>, FString> Deserialize(const TSharedRef<FJsonValue> &Value)
            requires TJsonDeserializable<T>
        {
            if (Value->IsNull())
            {
                return MakeValue(NullOpt);
            }

            auto DeserializedValue = TJsonConverter<T>::Deserialize(Value);
            if (auto *Error = DeserializedValue.TryGetError(); Error != nullptr)
            {
                return MakeError(MoveTemp(*Error));
            }

            return MakeValue(MoveTemp(DeserializedValue.GetValue()));
        }

        static TSharedRef<FJsonValue> Serialize(const TOptional<T> &Value)
            requires TJsonSerializable<T>
        {
            return Value.IsSet() ? TJsonConverter<T>::Serialize(Value.GetValue()) : MakeShared<FJsonValueNull>();
        }
    };

    template <TJsonDeserializable T>
    TValueOrError<T, FString> DeserializeFromJson(const TSharedRef<FJsonValue> &Value)
    {
        return TJsonConverter<T>::Deserialize(Value);
    }

    template <TJsonSerializable T>
    TSharedRef<FJsonValue> SerializeToJson(T &&Value)
    {
        return TJsonConverter<T>::Serialize(Forward<T>(Value));
    }

} // namespace PokeEdit