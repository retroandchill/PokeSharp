// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/EditorTabOption.h"

namespace PokeEdit
{
    TValueOrError<FEditorTabOption, FString> TJsonConverter<FEditorTabOption>::Deserialize(
        const TSharedRef<FJsonValue> &Value)
    {
        return TJsonObjectConverter<FEditorTabOption>::Deserialize(Value);
    }

    TSharedRef<FJsonValue> TJsonConverter<FEditorTabOption>::Serialize(const FEditorTabOption &Value)
    {
        return TJsonObjectConverter<FEditorTabOption>::Serialize(Value);
    }
} // namespace PokeEdit