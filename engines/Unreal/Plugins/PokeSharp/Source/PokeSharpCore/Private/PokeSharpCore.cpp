#include "PokeSharpCore.h"
#include "Configuration/SettingsChangeManager.h"
#include "Modules/ModuleManager.h"

#define LOCTEXT_NAMESPACE "FPokeSharpCoreModule"

void FPokeSharpCoreModule::StartupModule()
{
    USettingsChangeManager::Initialize();
}

void FPokeSharpCoreModule::ShutdownModule()
{
    USettingsChangeManager::Shutdown();
}

#undef LOCTEXT_NAMESPACE

IMPLEMENT_MODULE(FPokeSharpCoreModule, PokeSharpCore)