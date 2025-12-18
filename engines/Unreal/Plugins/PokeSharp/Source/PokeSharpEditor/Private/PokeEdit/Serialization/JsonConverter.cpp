// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Serialization/JsonConverter.h"
#include "JsonObjectConverter.h"
#include "Serialization/JsonSerializer.h"
#include "Serialization/JsonWriter.h"

namespace PokeEdit
{
    FString WriteAsString(const TSharedRef<FJsonValue> &Value)
    {
        FString Result;
        const auto Writer = TJsonWriterFactory<>::Create(&Result);
        FJsonSerializer::Serialize(Value, FString(), Writer);
        return Result;
    }

    std::expected<bool, FString> TJsonConverter<bool>::Deserialize(const TSharedRef<FJsonValue> &Value)
    {
        if (bool Result; Value->TryGetBool(Result))
        {
            return Result;
        }

        return std::unexpected(FString::Format(TEXT("Value '{0}' is not a boolean"), {WriteAsString(Value)}));
    }

    std::expected<FName, FString> TJsonConverter<FName>::Deserialize(const TSharedRef<FJsonValue> &Value)
    {
        if (FString Result; Value->TryGetString(Result))
        {
            return FName(Result);
        }

        return std::unexpected(FString::Format(TEXT("Value '{0}' is not a string"), {WriteAsString(Value)}));
    }

    std::expected<FString, FString> TJsonConverter<FString>::Deserialize(const TSharedRef<FJsonValue> &Value)
    {
        if (FString Result; Value->TryGetString(Result))
        {
            return Result;
        }

        return std::unexpected(FString::Format(TEXT("Value '{0}' is not a string"), {WriteAsString(Value)}));
    }

    std::expected<FText, FString> TJsonConverter<FText>::Deserialize(const TSharedRef<FJsonValue> &Value)
    {
        if (FString Result; Value->TryGetString(Result))
        {
            if (Result.IsEmpty())
            {
                return FText::GetEmpty();
            }

            FText LocalizedText;
            if (!FTextStringHelper::ReadFromBuffer(Result.GetCharArray().GetData(), LocalizedText))
            {
                LocalizedText = FText::FromString(Result);
            }
            return MoveTemp(LocalizedText);
        }

        return std::unexpected(FString::Format(TEXT("Value '{0}' is not a string"), {WriteAsString(Value)}));
    }

    TSharedRef<FJsonValue> TJsonConverter<FText>::Serialize(const FText &Value)
    {
        FString Buffer;
        FTextStringHelper::WriteToBuffer(Buffer, Value);
        return MakeShared<FJsonValueString>(MoveTemp(Buffer));
    }
} // namespace PokeEdit