// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "JsonSchema.h"

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

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FEntityRequest>,
                            std::make_tuple(TJsonField<&FEntityRequest::EditorId>(TEXT("editorId")),
                                            TJsonField<&FEntityRequest::Index>(TEXT("index"))));
    };

    template struct POKESHARPEDITOR_API TJsonConverter<FEntityRequest>;
} // namespace PokeEdit
