// Fill out your copyright notice in the Description page of Project Settings.

#include "Interop/PokeSharpSaveGameExporter.h"
#include "Saving/PokeSharpSaveGame.h"

void UPokeSharpSaveGameExporter::GetDataReadBuffer(const UPokeSharpSaveGame *SaveGame, const uint8 *&OutBuffer,
                                                   int32 &OutSize)
{
    const auto DataView = SaveGame->GetData();
    OutBuffer = DataView.GetData();
    OutSize = DataView.Num();
}

void UPokeSharpSaveGameExporter::WriteToDataBuffer(UPokeSharpSaveGame *SaveGame, const uint8 *Buffer, const int32 Size)
{
    // ReSharper disable once CppTemplateArgumentsCanBeDeduced
    const auto ArrayView = TConstArrayView<uint8>(Buffer, Size);
    SaveGame->WriteData(ArrayView);
}