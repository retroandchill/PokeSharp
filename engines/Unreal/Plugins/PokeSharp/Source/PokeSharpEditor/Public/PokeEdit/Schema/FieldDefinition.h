// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "OptionSourceDefinition.h"
#include "JsonSerializer.h"

namespace PokeEdit
{
    enum class EFieldKind : uint8
    {
        Text,
        Int,
        Float,
        Bool,
        Choice,
        Object,
        List,
        Dictionary,
        Optional
    };
    
    POKESHARPEDITOR_API FString LexToString(EFieldKind Kind);
    POKESHARPEDITOR_API bool LexFromString(EFieldKind &OutKind, FStringView Lex);
    POKESHARPEDITOR_API TOptional<EFieldKind> LexFromString(FStringView Lex);

    /**
     * 
     */
    struct FFieldDefinition
    {
        virtual ~FFieldDefinition() = default;
        virtual EFieldKind GetKind() const = 0;
    
        FName FieldId;
        FText Label;
        FText Tooltip;
        FName Category;
    };

    struct FBoolFieldDefinition final : FFieldDefinition
    {
        EFieldKind GetKind() const override { return EFieldKind::Bool; }
    };

    struct FTextFieldDefinition final : FFieldDefinition
    {
        EFieldKind GetKind() const override { return EFieldKind::Text; }
        TOptional<int32> MaxLength;
        TOptional<FString> Regex;
        bool AllowEmpty = true;
        bool AllowMultiline = false;
        bool IsLocalizable = false;
    };

    struct FIntFieldDefinition final : FFieldDefinition
    {
        EFieldKind GetKind() const override { return EFieldKind::Int; }
    
        TOptional<int32> MinValue;
        TOptional<int32> MaxValue;
        TOptional<int32> Step;
        TOptional<int32> DecimalPlaces;
    };

    struct FFloatFieldDefinition final : FFieldDefinition
    {
        EFieldKind GetKind() const override { return EFieldKind::Float; }
    
        TOptional<double> MinValue;
        TOptional<double> MaxValue;
        TOptional<double> Step;
    };

    struct FChoiceFieldDefinition final : FFieldDefinition
    {
        EFieldKind GetKind() const override { return EFieldKind::Choice; }
    
        bool AllowNone = false;
        FOptionSourceDefinition Options;
    };

    struct FObjectFieldDefinition final : FFieldDefinition
    {
        EFieldKind GetKind() const override { return EFieldKind::Object; }
    
        FName ObjectTypeId;
    };

    struct FCollectionFieldDefinition : FFieldDefinition
    {
        bool FixedSize = false;
        TOptional<int32> MinSize;
        TOptional<int32> MaxSize;
    };

    struct FListFieldDefinition final : FCollectionFieldDefinition
    {
        EFieldKind GetKind() const override { return EFieldKind::List; } 
    
        TSharedRef<FFieldDefinition> ItemDefinition;
    };

    struct FDictionaryFieldDefinition final : FCollectionFieldDefinition
    {
        EFieldKind GetKind() const override { return EFieldKind::Dictionary; } 
    
        TSharedRef<FFieldDefinition> KeyDefinition;
        TSharedRef<FFieldDefinition> ValueDefinition;
    };

    struct FOptionalFieldDefinition final : FFieldDefinition
    {
        EFieldKind GetKind() const override { return EFieldKind::Optional; } 
    
        TSharedRef<FFieldDefinition> ValueField;
    };
    
    template <>
    struct TJsonConverter<TSharedRef<FFieldDefinition>>
    {
        POKESHARPEDITOR_API static TValueOrError<TSharedRef<FFieldDefinition>, FString> Deserialize(const TSharedRef<FJsonValue>& Value);
        
        POKESHARPEDITOR_API static TSharedRef<FJsonValue> Serialize(TSharedRef<FFieldDefinition> Value);
    };
}