// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/OptionSourceDefinition.h"

namespace PokeEdit
{
    TValueOrError<FOptionItemDefinition, FString> TJsonConverter<FOptionItemDefinition>::Deserialize(
        const TSharedRef<FJsonValue> &Value)
    {
        return TJsonObjectConverter<FOptionItemDefinition>::Deserialize(Value);
    }

    TSharedRef<FJsonValue> TJsonConverter<FOptionItemDefinition>::Serialize(const FOptionItemDefinition &Value)
    {
        return TJsonObjectConverter<FOptionItemDefinition>::Serialize(Value);
    }

    TValueOrError<FStaticOptionSourceDefinition, FString> TJsonConverter<FStaticOptionSourceDefinition>::Deserialize(
        const TSharedRef<FJsonValue> &Value)
    {
        return TJsonObjectConverter<FStaticOptionSourceDefinition>::Deserialize(Value);
    }

    TSharedRef<FJsonValue> TJsonConverter<FStaticOptionSourceDefinition>::Serialize(
        const FStaticOptionSourceDefinition &Value)
    {
        return TJsonObjectConverter<FStaticOptionSourceDefinition>::Serialize(Value);
    }

    TValueOrError<FDynamicOptionSourceDefinition, FString> TJsonConverter<FDynamicOptionSourceDefinition>::Deserialize(
        const TSharedRef<FJsonValue> &Value)
    {
        return TJsonObjectConverter<FDynamicOptionSourceDefinition>::Deserialize(Value);
    }

    TSharedRef<FJsonValue> TJsonConverter<FDynamicOptionSourceDefinition>::Serialize(
        const FDynamicOptionSourceDefinition &Value)
    {
        return TJsonObjectConverter<FDynamicOptionSourceDefinition>::Serialize(Value);
    }
    
    TValueOrError<FOptionSourceDefinition, FString> TJsonConverter<FOptionSourceDefinition>::Deserialize(const TSharedRef<FJsonValue> &Value)
    {
        return TJsonUnionConverter<FOptionSourceDefinition>::Deserialize(Value);
    }

    TSharedRef<FJsonValue> TJsonConverter<FOptionSourceDefinition>::Serialize(const FOptionSourceDefinition &Value)
    {
        return TJsonUnionConverter<FOptionSourceDefinition>::Serialize(Value);
    }
} // namespace PokeEdit