// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSBindsManager.h"
#include "OptionSelectionCallbacks.h"
#include "PokeEditCallbacks.h"
#include "UObject/Object.h"

#include "PokeSharpEditorCallbacksExporter.generated.h"

/**
 *
 */
UCLASS()
class POKESHARPEDITOR_API UPokeSharpEditorCallbacksExporter : public UObject
{
    GENERATED_BODY()

  public:
    UNREALSHARP_FUNCTION()
    static void SetOptionSelectionCallbacks(FOptionSelectionCallbacks Callbacks);

    UNREALSHARP_FUNCTION()
    static void SetPokeEditCallbacks(FPokeEditCallbacks Callbacks);
};
