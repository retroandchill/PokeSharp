#pragma once

#include "CoreMinimal.h"

class FPokeSharpCoreModule : public IModuleInterface
{
  public:
    void StartupModule() override;
    void ShutdownModule() override;
};
