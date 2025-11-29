// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "JsonSchema.h"

namespace PokeEdit
{
    struct FOptionItemDefinition
    {
        FName Key;
        FText Label;

        static constexpr auto JsonSchema = TJsonObjectType(std::in_place_type<FOptionItemDefinition>, TJsonField<&FOptionItemDefinition::Key>(TEXT("key")),
                                                           TJsonField<&FOptionItemDefinition::Label>(TEXT("label")));
    };

    struct FStaticOptionSourceDefinition final
    {
        TArray<FOptionItemDefinition> Options;

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FStaticOptionSourceDefinition>, TJsonField<&FStaticOptionSourceDefinition::Options>(TEXT("options")));
    };

    struct FDynamicOptionSourceDefinition final
    {
        FName SourceId;

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FDynamicOptionSourceDefinition>, TJsonField<&FDynamicOptionSourceDefinition::SourceId>(TEXT("sourceId")));
    };

    using FOptionSourceDefinition = TVariant<FStaticOptionSourceDefinition, FDynamicOptionSourceDefinition>;
    
    template <>
    struct POKESHARPEDITOR_API TJsonConverter<FOptionItemDefinition>
    {
        static TValueOrError<FOptionItemDefinition, FString> Deserialize(const TSharedRef<FJsonValue> &Value);

        static TSharedRef<FJsonValue> Serialize(const FOptionItemDefinition &Value);
    };
    
    template <>
    struct POKESHARPEDITOR_API TJsonConverter<FStaticOptionSourceDefinition>
    {
        static TValueOrError<FStaticOptionSourceDefinition, FString> Deserialize(const TSharedRef<FJsonValue> &Value);

        static TSharedRef<FJsonValue> Serialize(const FStaticOptionSourceDefinition &Value);
    };
    
    template <>
    struct POKESHARPEDITOR_API TJsonConverter<FDynamicOptionSourceDefinition>
    {
        static TValueOrError<FDynamicOptionSourceDefinition, FString> Deserialize(const TSharedRef<FJsonValue> &Value);

        static TSharedRef<FJsonValue> Serialize(const FDynamicOptionSourceDefinition &Value);
    };
    
    template<>
    struct TJsonUnionTraits<FOptionSourceDefinition>
    {
        static constexpr auto JsonSchema = TJsonUnionType(TJsonDiscriminator<&FOptionSourceDefinition::GetIndex>(), 
            TJsonUnionKey<FStaticOptionSourceDefinition, FOptionSourceDefinition::IndexOfType<FStaticOptionSourceDefinition>()>(TEXT("Static")), 
            TJsonUnionKey<FDynamicOptionSourceDefinition, FOptionSourceDefinition::IndexOfType<FDynamicOptionSourceDefinition>()>(TEXT("Dynamic")));
    };
    
    template <>
    struct POKESHARPEDITOR_API TJsonConverter<FOptionSourceDefinition>
    {
        static TValueOrError<FOptionSourceDefinition, FString> Deserialize(const TSharedRef<FJsonValue> &Value);

        static TSharedRef<FJsonValue> Serialize(const FOptionSourceDefinition &Value);
    };
} // namespace PokeEdit