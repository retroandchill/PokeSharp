// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PokeEdit/Serialization/JsonMacros.h"

namespace PokeEdit
{
    /**
     *
     */
    struct FEntityRequest
    {

        FName EditorId;
        int32 Index;

        explicit(false) FEntityRequest(const FName InEditorId, const int32 InIndex)
            : EditorId(InEditorId), Index(InIndex)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FEntityRequest)
        JSON_FIELD_REQUIRED(EditorId)
        JSON_FIELD_REQUIRED(Index)
    JSON_OBJECT_SCHEMA_END

    template struct POKESHARPEDITOR_API TJsonConverter<FEntityRequest>;
} // namespace PokeEdit
