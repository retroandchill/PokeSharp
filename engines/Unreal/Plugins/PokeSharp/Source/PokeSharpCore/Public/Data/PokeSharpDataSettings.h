// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/DeveloperSettings.h"

#include "PokeSharpDataSettings.generated.h"

/**
 * The configuration regarding the pathing to search for the game's content.
 */
UCLASS(Config = Game, DefaultConfig)
class POKESHARPCORE_API UPokeSharpDataSettings : public UDeveloperSettings
{
    GENERATED_BODY()

  public:
    /**
     * The engine path that contains all the data files.
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "FilePaths", meta = (ContentDir))
    FDirectoryPath DataPath;

  protected:
#if WITH_EDITOR
    FText GetSectionText() const override;
#endif
};
