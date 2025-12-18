// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "DiffNode.h"
#include "PokeEdit/Serialization/JsonSchemaFwd.h"

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

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FEditorTabOption);

    struct FEntityUpdateResponse
    {
        TOptional<FObjectDiffNode> Diff;
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FEntityUpdateResponse);
} // namespace PokeEdit