// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "DiffNode.h"
#include "PokeEdit/Serialization/JsonMacros.h"

namespace PokeEdit
{
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

    struct FEntityUpdateRequest
    {
        FName EditorId;
        int32 Index;
        FObjectDiffNode Change;
        
        FEntityUpdateRequest(const FName InEditorId, const int32 InIndex, FObjectDiffNode InChange)
            : EditorId(InEditorId), Index(InIndex), Change(MoveTemp(InChange))
        {
        }
    };
    
    JSON_OBJECT_SCHEMA_BEGIN(FEntityUpdateRequest)
        JSON_FIELD_REQUIRED(EditorId)
        JSON_FIELD_REQUIRED(Index)
        JSON_FIELD_REQUIRED(Change)
    JSON_OBJECT_SCHEMA_END
    
    template struct POKESHARPEDITOR_API TJsonConverter<FEditorLabelRequest>;
    template struct POKESHARPEDITOR_API TJsonConverter<FEntityRequest>;
    template struct POKESHARPEDITOR_API TJsonConverter<FEntityUpdateRequest>;
} // namespace PokeEdit
