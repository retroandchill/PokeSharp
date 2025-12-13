// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/Responses.h"

namespace PokeEdit
{
    template struct TJsonConverter<FEditorTabOption>;
    template struct TJsonConverter<FEntityUpdateResponse>;
} // namespace PokeEdit