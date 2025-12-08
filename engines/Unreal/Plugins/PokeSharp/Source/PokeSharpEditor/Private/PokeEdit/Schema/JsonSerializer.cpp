// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/JsonSerializer.h"
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

    std::expected<TSharedRef<FStructOnScope>, FText> DeserializeFromJson(const TSharedRef<FJsonValue> &Value,
                                                                         const UStruct *Struct,
                                                                         const int64 CheckFlags,
                                                                         const int64 SkipFlags,
                                                                         const bool bStrictMode,
                                                                         const FCustomImportCallback *ImportCb)
    {
        const auto JsonObject = Value->AsObject();
        if (JsonObject == nullptr)
        {
            return std::unexpected(FText::FromStringView(TEXT("Provided value is not a JSON object")));
        }

        auto Result = MakeShared<FStructOnScope>(Struct);
        if (FText OutError; !FJsonObjectConverter::JsonObjectToUStruct(JsonObject.ToSharedRef(),
                                                                       Struct,
                                                                       Result->GetStructMemory(),
                                                                       CheckFlags,
                                                                       SkipFlags,
                                                                       bStrictMode,
                                                                       &OutError,
                                                                       ImportCb))
        {
            return std::unexpected(MoveTemp(OutError));
        }

        return Result;
    }
} // namespace PokeEdit