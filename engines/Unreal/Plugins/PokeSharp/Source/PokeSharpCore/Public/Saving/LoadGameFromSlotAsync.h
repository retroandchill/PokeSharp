// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UnrealSharpAsync/Public/CSAsyncActionBase.h"

#include "LoadGameFromSlotAsync.generated.h"

class USaveGame;
/**
 *
 */
UCLASS(meta = (InternalType))
class POKESHARPCORE_API ULoadGameFromSlotAsync : public UCSAsyncActionBase
{
    GENERATED_BODY()

  public:
    UFUNCTION(meta = (ScriptMethod))
    void LoadGameFromSlot(const FString &SlotName, int32 UserIndex);

  private:
    UPROPERTY()
    TObjectPtr<USaveGame> SaveGame;
};
