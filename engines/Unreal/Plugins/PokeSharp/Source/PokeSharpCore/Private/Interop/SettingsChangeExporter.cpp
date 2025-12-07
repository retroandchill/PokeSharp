// Fill out your copyright notice in the Description page of Project Settings.

#include "Interop/SettingsChangeExporter.h"
#include "Configuration/SettingsChangeManager.h"

void USettingsChangeExporter::RegisterSettingsChangeCallback(UDeveloperSettings *DeveloperSettings,
                                                             const FGCHandleIntPtr Callback,
                                                             FGuid &OutGuid)
{
    check(DeveloperSettings != nullptr);
    OutGuid = USettingsChangeManager::Get().Bind(DeveloperSettings, FGCHandle(Callback, GCHandleType::StrongHandle));
}

void USettingsChangeExporter::UnregisterSettingsChangeCallback(const FGuid &Guid)
{
    USettingsChangeManager::Get().Unbind(Guid);
}