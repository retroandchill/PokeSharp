// Fill out your copyright notice in the Description page of Project Settings.


#include "Interop/PokeSharpEditorCallbacksExporter.h"

void UPokeSharpEditorCallbacksExporter::SetOptionSelectionCallbacks(const FOptionSelectionCallbacks Callbacks)
{
    FOptionSelectionManager::Get().SetCallbacks(Callbacks);
}