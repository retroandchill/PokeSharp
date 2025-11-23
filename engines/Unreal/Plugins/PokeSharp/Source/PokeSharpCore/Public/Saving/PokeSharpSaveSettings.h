// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/DeveloperSettings.h"
#include "PokeSharpSaveGame.h"

#include "PokeSharpSaveSettings.generated.h"

class UPokeSharpSaveGame;
/**
 *
 */
UCLASS(Config = Game, DefaultConfig, meta = (NullableEnable))
class POKESHARPCORE_API UPokeSharpSaveSettings : public UDeveloperSettings
{
    GENERATED_BODY()

  public:
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Save Game")
    FString SaveSlotName = "Save";

    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Save Game")
    TSubclassOf<UPokeSharpSaveGame> SaveGameClass = UPokeSharpSaveGame::StaticClass();
};
