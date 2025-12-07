#pragma once

#include "CoreMinimal.h"
#include "Modules/ModuleInterface.h"

class FPokeSharpCoreModule : public IModuleInterface
{
  public:
    void StartupModule() override;
    void ShutdownModule() override;
};
