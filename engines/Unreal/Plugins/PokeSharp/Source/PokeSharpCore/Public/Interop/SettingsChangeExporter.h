// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSBindsManager.h"
#include "CSManagedGCHandle.h"
#include "UObject/Object.h"

#include "SettingsChangeExporter.generated.h"

class UDeveloperSettings;
/**
 *
 */
UCLASS()
class POKESHARPCORE_API USettingsChangeExporter : public UObject
{
    GENERATED_BODY()

  public:
    UNREALSHARP_FUNCTION()
    static void RegisterSettingsChangeCallback(UDeveloperSettings *DeveloperSettings,
                                               FGCHandleIntPtr Callback,
                                               FGuid &OutGuid);

    UNREALSHARP_FUNCTION()
    static void UnregisterSettingsChangeCallback(const FGuid &Guid);
};
