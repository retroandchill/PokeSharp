// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/Requests.h"

namespace PokeEdit
{
    template struct TJsonConverter<FEditorLabelRequest>;
    template struct TJsonConverter<FEntityRequest>;
    template struct TJsonConverter<FEntityUpdateRequest>;
} // namespace PokeEdit