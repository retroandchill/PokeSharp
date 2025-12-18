// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/PokeEditApi.h"
#include "PokeEdit/PokeEditClient.h"

// ReSharper disable once CppUnusedIncludeDirective
#include "PokeEdit/Serialization/JsonConverterTemplates.h"

namespace PokeEdit
{
    static const FName ModuleName = "Editor";

    std::expected<TArray<FEditorTabOption>, FString> GetEditorTabs()
    {
        static FName RequestName = "GetEditorTabs";
        return SendRequest<TArray<FEditorTabOption>>(ModuleName, RequestName);
    }

    std::expected<TArray<FText>, FString> GetEntryLabels(const FName EditorId)
    {
        static FName RequestName = "GetEntryLabels";
        return SendRequest<TArray<FText>>(ModuleName, RequestName, EditorId);
    }

    std::expected<TSharedRef<FJsonValue>, FString> GetEntryAtIndex(const FName EditorId, const int32 Index)
    {
        static FName RequestName = "GetEntryAtIndex";
        return SendRequest<TSharedRef<FJsonValue>>(ModuleName, RequestName, EditorId, Index);
    }

    std::expected<FEntityUpdateResponse, FString> UpdateEntityAtIndex(const FName EditorId,
                                                                      const int32 Index,
                                                                      FObjectDiffNode DiffNode)
    {
        static FName RequestName = "UpdateEntityAtIndex";
        return SendRequest<FEntityUpdateResponse>(ModuleName, RequestName, EditorId, Index, MoveTemp(DiffNode));
    }
} // namespace PokeEdit