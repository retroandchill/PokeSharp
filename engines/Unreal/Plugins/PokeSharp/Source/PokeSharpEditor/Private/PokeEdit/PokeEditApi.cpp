// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/PokeEditApi.h"
#include "PokeEdit/PokeEditClient.h"
#include "PokeEdit/Schema/EditorLabelRequest.h"
#include "PokeEdit/Schema/EntityRequest.h"
#include "PokeEdit/Schema/FieldDefinition.h"
#include "PokeEdit/Schema/FieldPath.h"

namespace PokeEdit
{
    const TSharedRef<FJsonValue> NoBodyJsonValue = MakeShared<FJsonValueObject>(MakeShared<FJsonObject>());

    std::expected<TArray<FEditorTabOption>, FString> GetEditorTabs()
    {
        static FName RequestName = "GetEditorTabs";

        return SendRequest(RequestName, NoBodyJsonValue)
            .and_then([](const TSharedRef<FJsonValue> &Response)
                      { return DeserializeFromJson<TArray<FEditorTabOption>>(Response); });
    }

    std::expected<TArray<FText>, FString> GetEntryLabels(const FName EditorId)
    {
        static FName RequestName = "GetEntryLabels";

        const auto JsonRequest = SerializeToJson(FEditorLabelRequest(EditorId));
        return SendRequest(RequestName, JsonRequest)
            .and_then([](const TSharedRef<FJsonValue> &Response)
                      { return DeserializeFromJson<TArray<FText>>(Response); });
    }

    std::expected<TSharedRef<FJsonValue>, FString> GetEntryAtIndex(const FName EditorId, const int32 Index)
    {
        static FName RequestName = "GetEntryAtIndex";
        const auto JsonRequest = SerializeToJson(FEntityRequest(EditorId, Index));
        return SendRequest(RequestName, JsonRequest);
    }

    std::expected<TSharedRef<FFieldDefinition>, FString> GetFieldDefinition(const FFieldPath &Path)
    {
        static FName RequestName = "GetFieldDefinition";

        const auto JsonRequest = SerializeToJson(Path);
        return SendRequest(RequestName, JsonRequest)
            .and_then([](const TSharedRef<FJsonValue> &Response)
                      { return DeserializeFromJson<TSharedRef<FFieldDefinition>>(Response); });
    }
} // namespace PokeEdit