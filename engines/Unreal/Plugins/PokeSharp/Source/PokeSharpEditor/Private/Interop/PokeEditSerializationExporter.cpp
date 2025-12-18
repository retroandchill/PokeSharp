// Fill out your copyright notice in the Description page of Project Settings.

#include "Interop/PokeEditSerializationExporter.h"

void UPokeEditSerializationExporter::SerializeString(const TCHAR *Buffer, const int32 Length, FString &Output)
{
    Output = FString(FStringView(Buffer, Length));
}

void UPokeEditSerializationExporter::SerializeByteArray(const uint8 *Buffer, const int32 Length, TArray<uint8> &Output)
{
    Output.SetNumZeroed(Length);
    FMemory::Memcpy(Output.GetData(), Buffer, Length);
}