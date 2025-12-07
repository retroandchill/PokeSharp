// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/PokeEditClient.h"
#include "Interop/PokeEditCallbacks.h"
#include "Serialization/ArrayWriter.h"
#include "Serialization/JsonSerializer.h"
#include "Serialization/JsonWriter.h"
#include "Serialization/MemoryReader.h"

namespace PokeEdit
{
    std::expected<TSharedRef<FJsonValue>, FString> SendRequest(const FName RequestName,
                                                               const TSharedRef<FJsonValue> &Payload)
    {
        TArray<uint8> Buffer;
        FMemoryWriter Writer(Buffer);
        if (const auto JsonWriter = TJsonWriter<UTF8CHAR>::Create(&Writer);
            !FJsonSerializer::Serialize(Payload, FString(), JsonWriter))
        {
            return std::unexpected(
                FString::Format(TEXT("Failed to serialize payload for request '{0}'"), {RequestName.ToString()}));
        }

        const auto RequestArchive = MakeShared<FMemoryReader>(Buffer);
        auto ResponseWriter = MakeShared<FArrayWriter>();

        return FPokeEditManager::Get()
            .SendRequest(RequestName, RequestArchive, ResponseWriter)
            .and_then([&ResponseWriter] {
                FMemoryReader Reader(*ResponseWriter);
                const auto JsonReader = TJsonReader<UTF8CHAR>::Create(&Reader);
                TSharedPtr<FJsonValue> JsonValue;
                if (!FJsonSerializer::Deserialize(JsonReader, JsonValue))
                {
                    return std::expected<TSharedRef<FJsonValue>, FString>(std::unexpect,
                                                                          TEXT("Failed to deserialize response"));
                }

                return std::expected<TSharedRef<FJsonValue>, FString>(JsonValue.ToSharedRef());
            });
    }
} // namespace PokeEdit