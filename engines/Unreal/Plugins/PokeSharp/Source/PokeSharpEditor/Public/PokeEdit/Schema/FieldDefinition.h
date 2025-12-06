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
        FText Category;

        bool IsDefaultValue = false;

      protected:
        FFieldDefinition(const FName InFieldId, FText InLabel) : FieldId(InFieldId), Label(MoveTemp(InLabel))
        {
        }

        constexpr static auto BaseFieldsRequired =
            std::make_tuple(TJsonField<&FFieldDefinition::FieldId>(TEXT("fieldId")),
                            TJsonField<&FFieldDefinition::Label>(TEXT("label")));

        constexpr static auto BaseFieldsOptional =
            std::make_tuple(TJsonField<&FFieldDefinition::Tooltip>(TEXT("tooltip")),
                            TJsonField<&FFieldDefinition::Category>(TEXT("category")),
                            TJsonField<&FFieldDefinition::IsDefaultValue>(TEXT("isDefaultValue")));
    };

    struct FBoolFieldDefinition final : FFieldDefinition
    {
        bool CurrentValue = false;

        FBoolFieldDefinition(const FName InFieldId, FText InLabel) : FFieldDefinition(InFieldId, MoveTemp(InLabel))
        {
        }

        EFieldKind GetKind() const override
        {
            return EFieldKind::Bool;
        }

        constexpr static auto JsonSchema = TJsonObjectType(
            std::in_place_type<FBoolFieldDefinition>, BaseFieldsRequired,
            std::tuple_cat(BaseFieldsOptional,
                           std::make_tuple(TJsonField<&FBoolFieldDefinition::CurrentValue>(TEXT("currentValue")))));
    };

    struct FTextFieldDefinition final : FFieldDefinition
    {
        FTextFieldDefinition(const FName InFieldId, FText InLabel) : FFieldDefinition(InFieldId, MoveTemp(InLabel))
        {
        }

        EFieldKind GetKind() const override
        {
            return EFieldKind::Text;
        }

        TOptional<int32> MaxLength;
        TOptional<FString> Regex;
        bool AllowEmpty = true;
        bool AllowMultiline = false;
        bool IsLocalizable = false;
        FString CurrentValue;

        constexpr static auto JsonSchema = TJsonObjectType(
            std::in_place_type<FTextFieldDefinition>, BaseFieldsRequired,
            std::tuple_cat(BaseFieldsOptional,
                           std::make_tuple(TJsonField<&FTextFieldDefinition::MaxLength>(TEXT("maxLength")),
                                           TJsonField<&FTextFieldDefinition::Regex>(TEXT("regex")),
                                           TJsonField<&FTextFieldDefinition::AllowEmpty>(TEXT("allowEmpty")),
                                           TJsonField<&FTextFieldDefinition::AllowMultiline>(TEXT("allowMultiline")),
                                           TJsonField<&FTextFieldDefinition::IsLocalizable>(TEXT("isLocalizable")),
                                           TJsonField<&FTextFieldDefinition::CurrentValue>(TEXT("currentValue")))));
    };

    template <typename T>
        requires std::is_arithmetic_v<T>
    struct TNumberFieldDefinition final : FFieldDefinition
    {
        TNumberFieldDefinition(const FName InFieldId, FText InLabel) : FFieldDefinition(InFieldId, MoveTemp(InLabel))
        {
        }

        EFieldKind GetKind() const override
        {
            return EFieldKind::Int;
        }

        TOptional<T> MinValue;
        TOptional<T> MaxValue;
        TOptional<T> Step;
        TOptional<int32> DecimalPlaces;
        T CurrentValue = 0;

        constexpr static auto JsonSchema = TJsonObjectType(
            std::in_place_type<TNumberFieldDefinition>, BaseFieldsRequired,
            std::tuple_cat(BaseFieldsOptional,
                           std::make_tuple(TJsonField<&TNumberFieldDefinition::MinValue>(TEXT("minValue")),
                                           TJsonField<&TNumberFieldDefinition::MaxValue>(TEXT("maxValue")),
                                           TJsonField<&TNumberFieldDefinition::Step>(TEXT("step")),
                                           TJsonField<&TNumberFieldDefinition::DecimalPlaces>(TEXT("decimalPlaces")),
                                           TJsonField<&TNumberFieldDefinition::CurrentValue>(TEXT("currentValue")))));
    };

    struct FChoiceFieldDefinition final : FFieldDefinition
    {
        FChoiceFieldDefinition(const FName InFieldId, FText InLabel, FOptionSourceDefinition InOptions,
                               const TSharedRef<FJsonValue> &InCurrentValue)
            : FFieldDefinition(InFieldId, MoveTemp(InLabel)), Options(MoveTemp(InOptions)), CurrentValue(InCurrentValue)
        {
        }

        EFieldKind GetKind() const override
        {
            return EFieldKind::Choice;
        }

        bool AllowNone = false;
        FOptionSourceDefinition Options;
        TSharedRef<FJsonValue> CurrentValue;

        constexpr static auto JsonSchema = TJsonObjectType(
            std::in_place_type<FChoiceFieldDefinition>,
            std::tuple_cat(BaseFieldsRequired,
                           std::make_tuple(TJsonField<&FChoiceFieldDefinition::Options>(TEXT("options")),
                                           TJsonField<&FChoiceFieldDefinition::CurrentValue>(TEXT("currentValue")))),
            std::tuple_cat(BaseFieldsOptional,
                           std::make_tuple(TJsonField<&FChoiceFieldDefinition::AllowNone>(TEXT("allowNone")))));
    };

    struct FObjectFieldDefinition final : FFieldDefinition
    {
        FObjectFieldDefinition(const FName InFieldId, FText InLabel) : FFieldDefinition(InFieldId, MoveTemp(InLabel))
        {
        }

        EFieldKind GetKind() const override
        {
            return EFieldKind::Object;
        }

        TArray<TSharedRef<FFieldDefinition>> Fields;

        constexpr static auto JsonSchema = TJsonObjectType(
            std::in_place_type<FObjectFieldDefinition>, BaseFieldsRequired,
            std::tuple_cat(BaseFieldsOptional,
                           std::make_tuple(TJsonField<&FObjectFieldDefinition::Fields>(TEXT("fields")))));
    };

    struct FCollectionFieldDefinition : FFieldDefinition
    {
        bool FixedSize = false;
        TOptional<int32> MinSize;
        TOptional<int32> MaxSize;

      protected:
        FCollectionFieldDefinition(const FName InFieldId, FText InLabel)
            : FFieldDefinition(InFieldId, MoveTemp(InLabel))
        {
        }

        constexpr static auto BaseFieldsOptional =
            std::tuple_cat(FFieldDefinition::BaseFieldsOptional,
                           std::make_tuple(TJsonField<&FCollectionFieldDefinition::FixedSize>(TEXT("fixedSize")),
                                           TJsonField<&FCollectionFieldDefinition::MinSize>(TEXT("minSize")),
                                           TJsonField<&FCollectionFieldDefinition::MaxSize>(TEXT("maxSize"))));
    };

    struct FListFieldDefinition final : FCollectionFieldDefinition
    {
        FListFieldDefinition(const FName InFieldId, FText InLabel)
            : FCollectionFieldDefinition(InFieldId, MoveTemp(InLabel))
        {
        }

        EFieldKind GetKind() const override
        {
            return EFieldKind::List;
        }

        TArray<TSharedRef<FFieldDefinition>> ItemFields;

        constexpr static auto JsonSchema = TJsonObjectType(
            std::in_place_type<FListFieldDefinition>, BaseFieldsRequired,
            std::tuple_cat(BaseFieldsOptional,
                           std::make_tuple(TJsonField<&FListFieldDefinition::ItemFields>(TEXT("itemFields")))));
    };

    struct FDictionaryFieldPair
    {
        FDictionaryFieldPair(const TSharedRef<FFieldDefinition> &InKeyField,
                             const TSharedRef<FFieldDefinition> &InValueField)
            : KeyField(InKeyField), ValueField(InValueField)
        {
        }

        TSharedRef<FFieldDefinition> KeyField;
        TSharedRef<FFieldDefinition> ValueField;

        constexpr static auto JsonSchema =
            TJsonObjectType(std::in_place_type<FDictionaryFieldPair>,
                            std::make_tuple(TJsonField<&FDictionaryFieldPair::KeyField>(TEXT("keyField")),
                                            TJsonField<&FDictionaryFieldPair::KeyField>(TEXT("valueField"))));
    };

    struct FDictionaryFieldDefinition final : FCollectionFieldDefinition
    {
        FDictionaryFieldDefinition(const FName InFieldId, FText InLabel)
            : FCollectionFieldDefinition(InFieldId, MoveTemp(InLabel))
        {
        }

        EFieldKind GetKind() const override
        {
            return EFieldKind::Dictionary;
        }

        TArray<FDictionaryFieldPair> Pairs;

        constexpr static auto JsonSchema = TJsonObjectType(
            std::in_place_type<FDictionaryFieldDefinition>, BaseFieldsRequired,
            std::tuple_cat(BaseFieldsOptional,
                           std::make_tuple(TJsonField<&FDictionaryFieldDefinition::Pairs>(TEXT("pairs")))));
    };

    struct FOptionalFieldDefinition final : FFieldDefinition
    {
        FOptionalFieldDefinition(const FName InFieldId, FText InLabel) : FFieldDefinition(InFieldId, MoveTemp(InLabel))
        {
        }

        EFieldKind GetKind() const override
        {
            return EFieldKind::Optional;
        }

        TSharedPtr<FFieldDefinition> ValueField;

        constexpr static auto JsonSchema = TJsonObjectType(
            std::in_place_type<FOptionalFieldDefinition>, BaseFieldsRequired,
            std::tuple_cat(BaseFieldsOptional, std::make_tuple(TJsonField<&FOptionalFieldDefinition::ValueField>(
                                                   TEXT("valueDefinition")))));
    };

    template <>
    struct TJsonUnionTraits<FFieldDefinition>
    {
        static constexpr auto JsonSchema =
            TJsonUnionType(TJsonDiscriminator<&FFieldDefinition::GetKind>(),
                           TJsonUnionKey<FBoolFieldDefinition, EFieldKind::Text>(TEXT("Bool")),
                           TJsonUnionKey<FTextFieldDefinition, EFieldKind::Text>(TEXT("Text")),
                           TJsonUnionKey<TNumberFieldDefinition<int8>, EFieldKind::Text>(TEXT("Int8")),
                           TJsonUnionKey<TNumberFieldDefinition<int16>, EFieldKind::Text>(TEXT("Int16")),
                           TJsonUnionKey<TNumberFieldDefinition<int32>, EFieldKind::Text>(TEXT("Int32")),
                           TJsonUnionKey<TNumberFieldDefinition<int64>, EFieldKind::Text>(TEXT("Int64")),
                           TJsonUnionKey<TNumberFieldDefinition<uint8>, EFieldKind::Text>(TEXT("UInt8")),
                           TJsonUnionKey<TNumberFieldDefinition<uint16>, EFieldKind::Text>(TEXT("UInt16")),
                           TJsonUnionKey<TNumberFieldDefinition<uint32>, EFieldKind::Text>(TEXT("UInt32")),
                           TJsonUnionKey<TNumberFieldDefinition<uint64>, EFieldKind::Text>(TEXT("UInt64")),
                           TJsonUnionKey<TNumberFieldDefinition<float>, EFieldKind::Text>(TEXT("Float")),
                           TJsonUnionKey<TNumberFieldDefinition<double>, EFieldKind::Text>(TEXT("Double")),
                           TJsonUnionKey<FChoiceFieldDefinition, EFieldKind::Text>(TEXT("Choice")),
                           TJsonUnionKey<FObjectFieldDefinition, EFieldKind::Text>(TEXT("Object")),
                           TJsonUnionKey<FListFieldDefinition, EFieldKind::Text>(TEXT("List")),
                           TJsonUnionKey<FDictionaryFieldDefinition, EFieldKind::Text>(TEXT("Dictionary")),
                           TJsonUnionKey<FOptionalFieldDefinition, EFieldKind::Text>(TEXT("Optional")));
    };

    template <>
    struct TJsonConverter<TSharedRef<FFieldDefinition>>
    {
        POKESHARPEDITOR_API static TValueOrError<TSharedRef<FFieldDefinition>, FString> Deserialize(
            const TSharedRef<FJsonValue> &Value);

        POKESHARPEDITOR_API static TSharedRef<FJsonValue> Serialize(const TSharedRef<FFieldDefinition> &Value);
    };

    template <>
    struct TJsonConverter<FDictionaryFieldPair>
    {
        POKESHARPEDITOR_API static TValueOrError<FDictionaryFieldPair, FString> Deserialize(
            const TSharedRef<FJsonValue> &Value);

        POKESHARPEDITOR_API static TSharedRef<FJsonValue> Serialize(const FDictionaryFieldPair &Value);
    };

    template <std::derived_from<FFieldDefinition> T>
        requires(!std::same_as<T, FFieldDefinition>)
    struct TJsonConverter<TSharedRef<T>>
    {
        static TValueOrError<TSharedRef<T>, FString> Deserialize(const TSharedRef<FJsonValue> &Value);
        static TSharedRef<FJsonValue> Serialize(const TSharedRef<T> &Value);
    };

    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FBoolFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FTextFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<TNumberFieldDefinition<int8>>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<TNumberFieldDefinition<int16>>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<TNumberFieldDefinition<int32>>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<TNumberFieldDefinition<int64>>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<TNumberFieldDefinition<uint8>>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<TNumberFieldDefinition<uint16>>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<TNumberFieldDefinition<uint32>>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<TNumberFieldDefinition<uint64>>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<TNumberFieldDefinition<float>>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<TNumberFieldDefinition<double>>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FChoiceFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FObjectFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FListFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FDictionaryFieldDefinition>>;
    template struct POKESHARPEDITOR_API TJsonConverter<TSharedRef<FOptionalFieldDefinition>>;
} // namespace PokeEdit