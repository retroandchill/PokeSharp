// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

namespace PokeEdit
{
    struct FOptionItemDefinition
    {
        FName Key;
    
        FText Label;
    };

    struct FStaticOptionSourceDefinition final
    {
        TArray<FOptionItemDefinition> Options;
    };

    struct FDynamicOptionSourceDefinition final
    {
        FName SourceId;
    };

    using FOptionSourceDefinition = TVariant<FStaticOptionSourceDefinition, FDynamicOptionSourceDefinition>;
}