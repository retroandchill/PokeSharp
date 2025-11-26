// Fill out your copyright notice in the Description page of Project Settings.

#include "Selection/OptionSelectionSourceFunctions.h"
#include "Interop/OptionSelectionCallbacks.h"

namespace DataOptions
{
    static FName Species = FName("Species");
    static FName TrainerType = FName("TrainerType");
} // namespace DataOptions

TArray<FName> UOptionSelectionSourceFunctions::GetSpecies()
{
    return FOptionSelectionManager::Get().GetNamesList("Species");
}

TArray<FName> UOptionSelectionSourceFunctions::GetTrainerTypes()
{
    return FOptionSelectionManager::Get().GetNamesList("TrainerType");
}