// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "DiffNode.h"
#include "PokeEdit/Serialization/JsonMacros.h"
#include "PokeEdit/Serialization/JsonSchema.h"

namespace PokeEdit
{
    /**
     *
     */
    struct FEditorTabOption
    {
        FName Id;
        FText Name;
    };

    JSON_OBJECT_SCHEMA_BEGIN(FEditorTabOption)
        JSON_FIELD_REQUIRED(Id)
        JSON_FIELD_REQUIRED(Name)
    JSON_OBJECT_SCHEMA_END
    
    struct FEntityUpdateResponse
    {
        TOptional<FObjectDiffNode> Diff;
    };
    
    JSON_OBJECT_SCHEMA_BEGIN(FEntityUpdateResponse)
        JSON_FIELD_REQUIRED(Diff)
    JSON_OBJECT_SCHEMA_END

    template struct POKESHARPEDITOR_API TJsonConverter<FEditorTabOption>;
    template struct POKESHARPEDITOR_API TJsonConverter<FEntityUpdateResponse>;
} // namespace PokeEdit