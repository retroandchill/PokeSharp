// Fill out your copyright notice in the Description page of Project Settings.

#include "Saving/PokeSharpSaveGame.h"
#include "Saving/PokeSharpSaveSettings.h"

UPokeSharpSaveGame *UPokeSharpSaveGame::CreateSaveGame()
{
    return NewObject<UPokeSharpSaveGame>(GetTransientPackage(), GetDefault<UPokeSharpSaveSettings>()->SaveGameClass);
}

void UPokeSharpSaveGame::SetDataLength(const int32 NewLength)
{
    Data.SetNumZeroed(NewLength);
}

void UPokeSharpSaveGame::WriteData(const TConstArrayView<uint8> Buffer)
{
    Data.Append(Buffer.GetData(), Buffer.Num());
}