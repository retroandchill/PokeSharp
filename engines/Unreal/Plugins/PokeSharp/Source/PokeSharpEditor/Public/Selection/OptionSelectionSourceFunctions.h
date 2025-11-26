// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Kismet/BlueprintFunctionLibrary.h"

#include "OptionSelectionSourceFunctions.generated.h"

/**
 *
 */
UCLASS()
class POKESHARPEDITOR_API UOptionSelectionSourceFunctions : public UBlueprintFunctionLibrary
{
    GENERATED_BODY()

  public:
    UFUNCTION(BlueprintPure, BlueprintInternalUseOnly)
    static TArray<FName> GetSpecies();

    UFUNCTION(BlueprintPure, BlueprintInternalUseOnly)
    static TArray<FName> GetTrainerTypes();
};
