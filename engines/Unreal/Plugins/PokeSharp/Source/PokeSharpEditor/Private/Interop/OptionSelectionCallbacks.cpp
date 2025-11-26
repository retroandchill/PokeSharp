// Fill out your copyright notice in the Description page of Project Settings.

#include "Interop/OptionSelectionCallbacks.h"

FOptionSelectionManager &FOptionSelectionManager::Get()
{
    static FOptionSelectionManager Instance;
    return Instance;
}

void FOptionSelectionManager::SetCallbacks(const FOptionSelectionCallbacks NewCallbacks)
{
    Callbacks = NewCallbacks;
}

TArray<FName> FOptionSelectionManager::GetNamesList(const FName ClassName) const
{
    TArray<FName> Result;
    Callbacks.GetNamesList(ClassName, Result);
    return Result;
}