// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "JsonSerializer.h"
#include "OptionSourceDefinition.h"

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
        
    protected:
        FFieldDefinition(FName InFieldId, FText InLabel) : FieldId(InFieldId), Label(MoveTemp(InLabel)) {}
        
        constexpr static auto BaseFieldsRequired = std::make_tuple(
            TJsonField<&FFieldDefinition::FieldId>(TEXT("fieldId")),
            TJsonField<&FFieldDefinition::Label>(TEXT("label"))
        );
        
        constexpr static auto BaseFieldsOptional = std::make_tuple(
            TJsonField<&FFieldDefinition::Tooltip>(TEXT("tooltip")),
            TJsonField<&FFieldDefinition::Category>(TEXT("category"))
        );
    };

    struct FBoolFieldDefinition final : FFieldDefinition
    {
        FBoolFieldDefinition(const FName InFieldId, FText InLabel) : FFieldDefinition(InFieldId, MoveTemp(InLabel)) {}
        
        EFieldKind GetKind() const override
        {
            return EFieldKind::Bool;
        }
        
        constexpr static auto JsonSchema = TJsonObjectType(std::in_place_type<FBoolFieldDefinition>, BaseFieldsRequired, BaseFieldsOptional);
    };

    struct FTextFieldDefinition final : FFieldDefinition
    {
        FTextFieldDefinition(const FName InFieldId, FText InLabel) : FFieldDefinition(InFieldId, MoveTemp(InLabel)) {}
        
        EFieldKind GetKind() const override
        {
            return EFieldKind::Text;
        }
        
        TOptional<int32> MaxLength;
        TOptional<FString> Regex;
        bool AllowEmpty = true;
        bool AllowMultiline = false;
        bool IsLocalizable = false;
        
        constexpr static auto JsonSchema = TJsonObjectType(std::in_place_type<FTextFieldDefinition>, BaseFieldsRequired, std::tuple_cat(BaseFieldsOptional, std::make_tuple(
            TJsonField<&FTextFieldDefinition::MaxLength>(TEXT("maxLength")),
            TJsonField<&FTextFieldDefinition::Regex>(TEXT("regex")),
            TJsonField<&FTextFieldDefinition::AllowEmpty>(TEXT("allowEmpty")),
            TJsonField<&FTextFieldDefinition::AllowMultiline>(TEXT("allowMultiline")),
            TJsonField<&FTextFieldDefinition::IsLocalizable>(TEXT("isLocalizable"))
        )));
    };

    struct FIntFieldDefinition final : FFieldDefinition
    {
        FIntFieldDefinition(const FName InFieldId, FText InLabel) : FFieldDefinition(InFieldId, MoveTemp(InLabel)) {}
        
        EFieldKind GetKind() const override
        {
            return EFieldKind::Int;
        }

        TOptional<int32> MinValue;
        TOptional<int32> MaxValue;
        TOptional<int32> Step;
        TOptional<int32> DecimalPlaces;
        
        constexpr static auto JsonSchema = TJsonObjectType(std::in_place_type<FIntFieldDefinition>, BaseFieldsRequired, std::tuple_cat(BaseFieldsOptional, std::make_tuple(
            TJsonField<&FIntFieldDefinition::MinValue>(TEXT("minValue")),
            TJsonField<&FIntFieldDefinition::MaxValue>(TEXT("maxValue")),
            TJsonField<&FIntFieldDefinition::Step>(TEXT("step")),
            TJsonField<&FIntFieldDefinition::DecimalPlaces>(TEXT("decimalPlaces"))
        )));
    };

    struct FFloatFieldDefinition final : FFieldDefinition
    {
        FFloatFieldDefinition(const FName InFieldId, FText InLabel) : FFieldDefinition(InFieldId, MoveTemp(InLabel)) {}
        
        EFieldKind GetKind() const override
        {
            return EFieldKind::Float;
        }

        TOptional<double> MinValue;
        TOptional<double> MaxValue;
        TOptional<double> Step;
        
        constexpr static auto JsonSchema = TJsonObjectType(std::in_place_type<FFloatFieldDefinition>, BaseFieldsRequired, std::tuple_cat(BaseFieldsOptional, std::make_tuple(
            TJsonField<&FFloatFieldDefinition::MinValue>(TEXT("minValue")),
            TJsonField<&FFloatFieldDefinition::MaxValue>(TEXT("maxValue")),
            TJsonField<&FFloatFieldDefinition::Step>(TEXT("step"))
        )));
    };

    struct FChoiceFieldDefinition final : FFieldDefinition
    {
        FChoiceFieldDefinition(const FName InFieldId, FText InLabel, FOptionSourceDefinition InOptions) : FFieldDefinition(InFieldId, MoveTemp(InLabel)), Options(MoveTemp(InOptions)) {}
        
        EFieldKind GetKind() const override
        {
            return EFieldKind::Choice;
        }

        bool AllowNone = false;
        FOptionSourceDefinition Options;
        
        constexpr static auto JsonSchema = TJsonObjectType(std::in_place_type<FChoiceFieldDefinition>, 
            std::tuple_cat(BaseFieldsRequired, std::make_tuple(TJsonField<&FChoiceFieldDefinition::Options>(TEXT("options")))),
        std::tuple_cat(BaseFieldsOptional, std::make_tuple(
            TJsonField<&FChoiceFieldDefinition::AllowNone>(TEXT("allowNone"))
        )));
    };

    struct FObjectFieldDefinition final : FFieldDefinition
    {
        FObjectFieldDefinition(const FName InFieldId, FText InLabel, FName InObjectTypeId) : FFieldDefinition(InFieldId, MoveTemp(InLabel)), ObjectTypeId(InObjectTypeId) {}
        
        EFieldKind GetKind() const override
        {
            return EFieldKind::Object;
        }

        FName ObjectTypeId;
        
        constexpr static auto JsonSchema = TJsonObjectType(std::in_place_type<FObjectFieldDefinition>, std::tuple_cat(BaseFieldsRequired, std::make_tuple(
            TJsonField<&FObjectFieldDefinition::ObjectTypeId>(TEXT("objectTypeId"))
        )), BaseFieldsOptional);
    };

    struct FCollectionFieldDefinition : FFieldDefinition
    {
        bool FixedSize = false;
        TOptional<int32> MinSize;
        TOptional<int32> MaxSize;
        
    protected:
        FCollectionFieldDefinition(const FName InFieldId, FText InLabel) : FFieldDefinition(InFieldId, MoveTemp(InLabel)) {}
        
        constexpr static auto BaseFieldsOptional = std::tuple_cat(FFieldDefinition::BaseFieldsOptional, std::make_tuple(
            TJsonField<&FCollectionFieldDefinition::FixedSize>(TEXT("fixedSize")),
            TJsonField<&FCollectionFieldDefinition::MinSize>(TEXT("minSize")),
            TJsonField<&FCollectionFieldDefinition::MaxSize>(TEXT("maxSize"))
        ));
    };

    struct FListFieldDefinition final : FCollectionFieldDefinition
    {
        FListFieldDefinition(const FName InFieldId, FText InLabel, TSharedRef<FFieldDefinition> InItemField) : FCollectionFieldDefinition(InFieldId, MoveTemp(InLabel)), ItemField(MoveTemp(InItemField)) {}
        
        EFieldKind GetKind() const override
        {
            return EFieldKind::List;
        }

        TSharedRef<FFieldDefinition> ItemField;
        
        constexpr static auto JsonSchema = TJsonObjectType(std::in_place_type<FListFieldDefinition>, std::tuple_cat(BaseFieldsRequired, std::make_tuple(
            TJsonField<&FListFieldDefinition::ItemField>(TEXT("itemField"))
        )), BaseFieldsOptional);
    };

    struct FDictionaryFieldDefinition final : FCollectionFieldDefinition
    {
        FDictionaryFieldDefinition(const FName InFieldId, FText InLabel, TSharedRef<FFieldDefinition> InKeyField, TSharedRef<FFieldDefinition> InValueField) : FCollectionFieldDefinition(InFieldId, MoveTemp(InLabel)), KeyField(MoveTemp(InKeyField)), ValueField(MoveTemp(InValueField)) {}
        
        EFieldKind GetKind() const override
        {
            return EFieldKind::Dictionary;
        }

        TSharedRef<FFieldDefinition> KeyField;
        TSharedRef<FFieldDefinition> ValueField;
        
        constexpr static auto JsonSchema = TJsonObjectType(std::in_place_type<FDictionaryFieldDefinition>, std::tuple_cat(BaseFieldsRequired, std::make_tuple(
            TJsonField<&FDictionaryFieldDefinition::KeyField>(TEXT("keyField")),
            TJsonField<&FDictionaryFieldDefinition::ValueField>(TEXT("valueField"))
        )), BaseFieldsOptional);
    };

    struct FOptionalFieldDefinition final : FFieldDefinition
    {
        FOptionalFieldDefinition(const FName InFieldId, FText InLabel, TSharedRef<FFieldDefinition> InValueField) : FFieldDefinition(InFieldId, MoveTemp(InLabel)), ValueField(MoveTemp(InValueField)) {}
        
        EFieldKind GetKind() const override
        {
            return EFieldKind::Optional;
        }

        TSharedRef<FFieldDefinition> ValueField;
        
        constexpr static auto JsonSchema = TJsonObjectType(std::in_place_type<FOptionalFieldDefinition>, std::tuple_cat(BaseFieldsRequired, std::make_tuple(
            TJsonField<&FOptionalFieldDefinition::ValueField>(TEXT("valueDefinition"))
        )), BaseFieldsOptional);
    };
    
    template<>
    struct TJsonUnionTraits<FFieldDefinition>
    {
        static constexpr auto JsonSchema = TJsonUnionType(TJsonDiscriminator<&FFieldDefinition::GetKind>(),
            TJsonUnionKey<FBoolFieldDefinition, EFieldKind::Text>(TEXT("Bool")),
            TJsonUnionKey<FTextFieldDefinition, EFieldKind::Text>(TEXT("Text")),
            TJsonUnionKey<FIntFieldDefinition, EFieldKind::Text>(TEXT("Int")),
            TJsonUnionKey<FFloatFieldDefinition, EFieldKind::Text>(TEXT("Float")),
            TJsonUnionKey<FChoiceFieldDefinition, EFieldKind::Text>(TEXT("Choice")),
            TJsonUnionKey<FObjectFieldDefinition, EFieldKind::Text>(TEXT("Object")),
            TJsonUnionKey<FListFieldDefinition, EFieldKind::Text>(TEXT("List")),
            TJsonUnionKey<FDictionaryFieldDefinition, EFieldKind::Text>(TEXT("Dictionary")),
            TJsonUnionKey<FOptionalFieldDefinition, EFieldKind::Text>(TEXT("Optional"))
        );
    };
    
    template <>
    struct TJsonConverter<TSharedRef<FFieldDefinition>>
    {
        POKESHARPEDITOR_API static TValueOrError<TSharedRef<FFieldDefinition>, FString> Deserialize(
            const TSharedRef<FJsonValue> &Value);

        POKESHARPEDITOR_API static TSharedRef<FJsonValue> Serialize(const TSharedRef<FFieldDefinition> &Value);
    };
    
    template <std::derived_from<FFieldDefinition> T>
        requires (!std::same_as<T, FFieldDefinition>)
    struct TJsonConverter<TSharedRef<T>>
    {
        static TValueOrError<TSharedRef<T>, FString> Deserialize(const TSharedRef<FJsonValue> &Value);
        static TSharedRef<FJsonValue> Serialize(const TSharedRef<T> &Value);
    };

    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FBoolFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FTextFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FIntFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FFloatFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FChoiceFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FObjectFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FListFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FDictionaryFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FOptionalFieldDefinition>>;
} // namespace PokeEdit