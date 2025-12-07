// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/OptionSourceDefinition.h"

namespace PokeEdit
{
    template struct TJsonConverter<FOptionItemDefinition>;
    template struct TJsonConverter<FStaticOptionSourceDefinition>;
    template struct TJsonConverter<FDynamicOptionSourceDefinition>;
    template struct TJsonConverter<FOptionSourceDefinition>;
} // namespace PokeEdit