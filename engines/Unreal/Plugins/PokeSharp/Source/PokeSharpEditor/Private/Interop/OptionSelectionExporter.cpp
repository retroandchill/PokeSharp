// Fill out your copyright notice in the Description page of Project Settings.

#include "Interop/OptionSelectionExporter.h"

void UOptionSelectionExporter::SetArraySize(TArray<FName> &Names, const int32 Size)
{
    Names.SetNumZeroed(Size);
}