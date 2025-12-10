// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PokeEdit/Serialization/JsonMacros.h"
#include "PokeEdit/Serialization/JsonSchema.h"

namespace PokeEdit
{
    /**
     *
     */
    struct FEditorLabelRequest
    {
        explicit(false) FEditorLabelRequest(const FName InEditorId) : EditorId(InEditorId)
        {
        }

        FName EditorId;
    };

    JSON_OBJECT_SCHEMA_BEGIN(FEditorLabelRequest)
        JSON_FIELD_REQUIRED(EditorId)
    JSON_OBJECT_SCHEMA_END

    static_assert(TValidJsonObjectContainer<FEditorLabelRequest>);
    template struct POKESHARPEDITOR_API TJsonConverter<FEditorLabelRequest>;
} // namespace PokeEdit