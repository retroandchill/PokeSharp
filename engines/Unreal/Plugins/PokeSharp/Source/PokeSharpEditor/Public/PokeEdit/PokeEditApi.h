// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Schema/EditorTabOption.h"
#include "Schema/FieldEdit.h"

namespace PokeEdit
{
    struct FFieldPath;

    POKESHARPEDITOR_API std::expected<TArray<FEditorTabOption>, FString> GetEditorTabs();

    POKESHARPEDITOR_API std::expected<TArray<FText>, FString> GetEntryLabels(FName EditorId);

    POKESHARPEDITOR_API std::expected<TSharedRef<FJsonValue>, FString> GetEntryAtIndex(FName EditorId, int32 Index);

    POKESHARPEDITOR_API std::expected<TSharedRef<FJsonValue>, FString> GetEntryAtIndex(FName EditorId, int32 Index);
    
    POKESHARPEDITOR_API std::expected<TArray<FFieldEdit>, FString> PerformFieldEdit(const FFieldEdit& Edit);
} // namespace PokeEdit
