// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/PokeEditClient.h"
#include "Serialization/ArrayWriter.h"

TValueOrError<TSharedRef<FJsonValue>, FString> FPokeEditClient::SendRequest(const FName RequestName,
                                                                            const TSharedRef<FJsonValue> &Payload)
{
    TArray<uint8> Buffer;
    FMemoryWriter Writer(Buffer);
    auto JsonWriter = TJsonWriter<UTF8CHAR>::Create(&Writer);
    if (!FJsonSerializer::Serialize(Payload, FString(), JsonWriter))
    {
        return MakeError(
            FString::Format(TEXT("Failed to serialize payload for request '{0}'"), {RequestName.ToString()}));
    }

    auto RequestArchive = MakeShared<FMemoryReader>(Buffer);
    auto ResponseWriter = MakeShared<FArrayWriter>();
    // TODO: Add interop callback
}