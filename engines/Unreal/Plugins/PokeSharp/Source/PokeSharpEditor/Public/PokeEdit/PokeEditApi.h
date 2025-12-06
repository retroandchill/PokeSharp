// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Schema/EditorTabOption.h"

namespace PokeEdit
{
    POKESHARPEDITOR_API extern const TSharedRef<FJsonValue> NoBodyJsonValue;

    POKESHARPEDITOR_API TValueOrError<TArray<FEditorTabOption>, FString> GetEditorTabs();

    POKESHARPEDITOR_API TValueOrError<TArray<FText>, FString> GetEntryLabels(FName EditorId);
} // namespace PokeEdit
