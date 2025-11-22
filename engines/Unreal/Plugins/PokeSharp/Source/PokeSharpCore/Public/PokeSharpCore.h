#pragma once

#include "CoreMinimal.h"

class FPokeSharpCoreModule : public IModuleInterface
{
public:
    virtual void StartupModule() override;
    virtual void ShutdownModule() override;
};
