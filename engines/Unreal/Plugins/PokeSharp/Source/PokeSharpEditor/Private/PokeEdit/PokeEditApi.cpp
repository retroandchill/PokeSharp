// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/PokeEditApi.h"
#include "PokeEdit/PokeEditClient.h"
#include "PokeEdit/Schema/EditorLabelRequest.h"
#include "PokeEdit/Schema/EntityRequest.h"
#include "PokeEdit/Schema/FieldPath.h"

namespace PokeEdit
{
    std::expected<TArray<FEditorTabOption>, FString> GetEditorTabs()
    {
        static FName RequestName = "GetEditorTabs";
        return SendRequest<TArray<FEditorTabOption>>(RequestName);
    }

    std::expected<TArray<FText>, FString> GetEntryLabels(const FName EditorId)
    {
        static FName RequestName = "GetEntryLabels";
        return SendRequest<TArray<FText>>(RequestName, FEditorLabelRequest(EditorId));
    }

    std::expected<TSharedRef<FJsonValue>, FString> GetEntryAtIndex(const FName EditorId, const int32 Index)
    {
        static FName RequestName = "GetEntryAtIndex";
        return SendRequest<TSharedRef<FJsonValue>>(RequestName, FEntityRequest(EditorId, Index));
    }
} // namespace PokeEdit