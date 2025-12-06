// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "JsonSchema.h"

namespace PokeEdit
{
    /**
     * 
     */
    struct FEditorLabelRequest
    {
        explicit(false) FEditorLabelRequest(const FName InEditorId) : EditorId(InEditorId) { }
        
        FName EditorId;
        
        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FEditorLabelRequest>,
                            std::make_tuple(TJsonField<&FEditorLabelRequest::EditorId>(TEXT("editorId"))));
    };
    
    template <>
    struct POKESHARPEDITOR_API TJsonConverter<FEditorLabelRequest>
    {
        static TValueOrError<FEditorLabelRequest, FString> Deserialize(const TSharedRef<FJsonValue> &Value);

        static TSharedRef<FJsonValue> Serialize(const FEditorLabelRequest &Value);
    };
}
