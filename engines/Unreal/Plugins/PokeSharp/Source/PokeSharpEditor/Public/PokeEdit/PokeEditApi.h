// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Schema/EditorTabOption.h"

namespace PokeEdit
{
    struct FFieldDefinition;
    struct FFieldPath;

    POKESHARPEDITOR_API extern const TSharedRef<FJsonValue> NoBodyJsonValue;

    POKESHARPEDITOR_API std::expected<TArray<FEditorTabOption>, FString> GetEditorTabs();

    POKESHARPEDITOR_API std::expected<TArray<FText>, FString> GetEntryLabels(FName EditorId);

    POKESHARPEDITOR_API std::expected<TSharedRef<FFieldDefinition>, FString> GetFieldDefinition(const FFieldPath &Path);
} // namespace PokeEdit
