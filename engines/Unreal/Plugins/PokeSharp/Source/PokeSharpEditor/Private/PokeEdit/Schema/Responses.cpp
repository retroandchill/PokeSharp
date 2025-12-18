// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/Responses.h"
#include "PokeEdit/Serialization/JsonSchema.h"

namespace PokeEdit
{
    JSON_OBJECT_SCHEMA_BEGIN(FEditorTabOption)
        JSON_FIELD_REQUIRED(Id)
        JSON_FIELD_REQUIRED(Name)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FEditorTabOption);

    JSON_OBJECT_SCHEMA_BEGIN(FEntityUpdateResponse)
        JSON_FIELD_REQUIRED(Diff)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FEntityUpdateResponse);
} // namespace PokeEdit