// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSBindsManager.h"
#include "UObject/Object.h"
#include "OptionSelectionExporter.generated.h"

/**
 * 
 */
UCLASS()
class POKESHARPEDITOR_API UOptionSelectionExporter : public UObject
{
    GENERATED_BODY()
    
public:
    UNREALSHARP_FUNCTION()
    static void SetArraySize(TArray<FName>& Names, int32 Size);

};
