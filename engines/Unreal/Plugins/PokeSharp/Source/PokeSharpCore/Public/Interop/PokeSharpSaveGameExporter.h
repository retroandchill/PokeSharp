// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSBindsManager.h"
#include "UObject/Object.h"

#include "PokeSharpSaveGameExporter.generated.h"

class UPokeSharpSaveGame;
/**
 *
 */
UCLASS()
class POKESHARPCORE_API UPokeSharpSaveGameExporter : public UObject
{
    GENERATED_BODY()

  public:
    UNREALSHARP_FUNCTION()
    static void GetDataReadBuffer(const UPokeSharpSaveGame *SaveGame, const uint8 *&OutBuffer, int32 &OutSize);

    UNREALSHARP_FUNCTION()
    static void WriteToDataBuffer(UPokeSharpSaveGame *SaveGame, const uint8 *Buffer, int32 Size);
};
