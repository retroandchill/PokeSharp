// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"

#include "PokeSharpGameMode.generated.h"

/**
 *  Simple GameMode for a third person game
 */
UCLASS(abstract)
class APokeSharpGameMode : public AGameModeBase
{
    GENERATED_BODY()

  public:
    /** Constructor */
    APokeSharpGameMode();
};
