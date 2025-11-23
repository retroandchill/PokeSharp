// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UnrealSharpAsync/Public/CSAsyncActionBase.h"

#include "SaveGameToSlotAsync.generated.h"

class USaveGame;

/**
 *
 */
UCLASS(meta = (InternalType))
class POKESHARPCORE_API USaveGameToSlotAsync : public UCSAsyncActionBase
{
    GENERATED_BODY()

  public:
    UFUNCTION(meta = (ScriptMethod))
    void SaveGameToSlot(const FString &SlotName, int32 UserIndex, USaveGame *SaveGame);

  private:
    UPROPERTY()
    bool bSucceeded = false;
};
