// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/PokeEditClient.h"
#include "Interop/PokeEditCallbacks.h"
#include "Serialization/ArrayWriter.h"

namespace PokeEdit
{
    TValueOrError<TSharedRef<FJsonValue>, FString> SendRequest(const FName RequestName,
                                                               const TSharedRef<FJsonValue> &Payload)
    {
        TArray<uint8> Buffer;
        FMemoryWriter Writer(Buffer);
        if (auto JsonWriter = TJsonWriter<UTF8CHAR>::Create(&Writer);
            !FJsonSerializer::Serialize(Payload, FString(), JsonWriter))
        {
            return MakeError(
                FString::Format(TEXT("Failed to serialize payload for request '{0}'"), {RequestName.ToString()}));
        }

        auto RequestArchive = MakeShared<FMemoryReader>(Buffer);
        auto ResponseWriter = MakeShared<FArrayWriter>();

        if (auto Result = FPokeEditManager::Get().SendRequest(RequestName, RequestArchive, ResponseWriter);
            Result.HasError())
        {
            return MakeError(Result.StealError());
        }

        FMemoryReader Reader(*ResponseWriter);
        auto JsonReader = TJsonReader<UTF8CHAR>::Create(&Reader);
        TSharedPtr<FJsonValue> JsonValue;
        if (!FJsonSerializer::Deserialize(JsonReader, JsonValue))
        {
            return MakeError(TEXT("Failed to deserialize response"));
        }

        return MakeValue(JsonValue.ToSharedRef());
    }
} // namespace PokeEdit