// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/JsonSerializer.h"
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

    TValueOrError<bool, FString> TJsonConverter<bool>::Deserialize(const TSharedRef<FJsonValue> &Value)
    {
        if (bool Result; Value->TryGetBool(Result))
        {
            return MakeValue(Result);
        }

        return MakeError(FString::Format(TEXT("Value '{0}' is not a boolean"), {WriteAsString(Value)}));
    }

    TValueOrError<FName, FString> TJsonConverter<FName>::Deserialize(const TSharedRef<FJsonValue> &Value)
    {
        if (FString Result; Value->TryGetString(Result))
        {
            return MakeValue(FName(Result));
        }

        return MakeError(FString::Format(TEXT("Value '{0}' is not a string"), {WriteAsString(Value)}));
    }

    TValueOrError<FString, FString> TJsonConverter<FString>::Deserialize(const TSharedRef<FJsonValue> &Value)
    {
        if (FString Result; Value->TryGetString(Result))
        {
            return MakeValue(Result);
        }

        return MakeError(FString::Format(TEXT("Value '{0}' is not a string"), {WriteAsString(Value)}));
    }

    TValueOrError<FText, FString> TJsonConverter<FText>::Deserialize(const TSharedRef<FJsonValue> &Value)
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

    TSharedRef<FJsonValue> TJsonConverter<FText>::Serialize(const FText &Value)
    {
        FString Buffer;
        FTextStringHelper::WriteToBuffer(Buffer, Value);
        return MakeShared<FJsonValueString>(MoveTemp(Buffer));
    }
} // namespace PokeEdit