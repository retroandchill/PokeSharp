#include "PokeEdit/Requests/RequestPacking.h"
#include "LogPokeSharpEditor.h"

namespace PokeEdit
{

    std::expected<TArray<uint8>, FString> WriteJsonToBuffer(const TSharedRef<FJsonValue> &JsonValue)
    {
        TArray<uint8> Buffer;
        FMemoryWriter Writer(Buffer);
        if (const auto JsonWriter = TJsonWriter<UTF8CHAR>::Create(&Writer);
            !FJsonSerializer::Serialize(JsonValue, FString(), JsonWriter))
        {
            return std::unexpected(FString(TEXT("Failed to serialize JSON value")));
        }
        
        return Buffer;
    }

    std::expected<TSharedRef<FJsonValue>, FString> ReadJsonFromBuffer(const TArray<uint8> &Buffer)
    {
        FMemoryReader Reader(Buffer);
        const auto JsonReader = TJsonReader<UTF8CHAR>::Create(&Reader);
        TSharedPtr<FJsonValue> JsonValue;
        if (!FJsonSerializer::Deserialize(JsonReader, JsonValue))
        {
            return std::expected<TSharedRef<FJsonValue>, FString>(std::unexpect,
                                                                  TEXT("Failed to deserialize response"));
        }

        return std::expected<TSharedRef<FJsonValue>, FString>(JsonValue.ToSharedRef());
    }
}