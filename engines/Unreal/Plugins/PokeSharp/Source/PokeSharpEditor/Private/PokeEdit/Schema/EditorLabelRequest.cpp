// Fill out your copyright notice in the Description page of Project Settings.


#include "PokeEdit/Schema/EditorLabelRequest.h"

namespace PokeEdit
{
    TValueOrError<FEditorLabelRequest, FString> TJsonConverter<FEditorLabelRequest>::Deserialize(const TSharedRef<FJsonValue> &Value)
    {
        return TJsonObjectConverter<FEditorLabelRequest>::Deserialize(Value);
    }

    TSharedRef<FJsonValue> TJsonConverter<FEditorLabelRequest>::Serialize(const FEditorLabelRequest &Value)
    {
        return TJsonObjectConverter<FEditorLabelRequest>::Serialize(Value);
    }
}