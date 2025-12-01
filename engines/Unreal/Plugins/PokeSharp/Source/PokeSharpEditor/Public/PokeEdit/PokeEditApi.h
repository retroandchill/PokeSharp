// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Schema/OptionSourceDefinition.h"

namespace PokeEdit
{
    POKESHARPEDITOR_API extern const TSharedRef<FJsonValue> NoBodyJsonValue;

    POKESHARPEDITOR_API TValueOrError<TArray<FOptionItemDefinition>, FString> GetEditorTabs();
} // namespace PokeEdit
