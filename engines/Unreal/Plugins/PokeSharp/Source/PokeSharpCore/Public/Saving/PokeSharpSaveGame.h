// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/SaveGame.h"

#include "PokeSharpSaveGame.generated.h"

/**
 *
 */
UCLASS()
class POKESHARPCORE_API UPokeSharpSaveGame : public USaveGame
{
    GENERATED_BODY()

  public:
    UFUNCTION(meta = (ScriptMethod))
    static UPokeSharpSaveGame *CreateSaveGame();

    TConstArrayView<uint8> GetData() const
    {
        return Data;
    }

    UFUNCTION(meta = (ScriptMethod))
    int32 GetDataSize() const
    {
        return Data.Num();
    }

    UFUNCTION(meta = (ScriptMethod))
    void SetDataLength(int32 NewLength);

    void WriteData(TConstArrayView<uint8> Buffer);

  private:
    UPROPERTY()
    TArray<uint8> Data;
};
