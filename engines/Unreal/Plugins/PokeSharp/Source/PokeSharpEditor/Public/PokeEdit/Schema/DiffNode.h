// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PokeEdit/Serialization/JsonMacros.h"
#include "PokeEdit/Serialization/VariantWrapper.h"

namespace PokeEdit
{
    struct FListEditNode;
    struct FDictionaryEditNode;
    struct FDiffNode;

    struct FValueSetNode
    {
        TSharedRef<FJsonValue> NewValue;

        explicit FValueSetNode(const TSharedRef<FJsonValue> &NewValue) : NewValue(NewValue)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FValueSetNode)
        JSON_FIELD_REQUIRED(NewValue)
    JSON_OBJECT_SCHEMA_END

    struct FValueResetNode
    {
    };

    JSON_OBJECT_SCHEMA_BEGIN(FValueResetNode)
    JSON_OBJECT_SCHEMA_END

    struct FObjectDiffNode
    {
        TMap<FName, TSharedRef<FDiffNode>> Edits;

        explicit FObjectDiffNode(TMap<FName, TSharedRef<FDiffNode>> Edits) : Edits(MoveTemp(Edits))
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FObjectDiffNode)
        JSON_FIELD_REQUIRED(Edits)
    JSON_OBJECT_SCHEMA_END

    struct FListDiffNode
    {
        TArray<FListEditNode> Edits;

        explicit FListDiffNode(TArray<FListEditNode> Edits) : Edits(MoveTemp(Edits))
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListDiffNode)
        JSON_FIELD_REQUIRED(Edits)
    JSON_OBJECT_SCHEMA_END

    struct FDictionaryDiffNode
    {
        TArray<FDictionaryEditNode> Edits;

        explicit FDictionaryDiffNode(TArray<FDictionaryEditNode> Edits) : Edits(MoveTemp(Edits))
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FDictionaryDiffNode)
        JSON_FIELD_REQUIRED(Edits)
    JSON_OBJECT_SCHEMA_END

    struct FDiffNode
        : TVariantWrapper<FValueSetNode, FValueResetNode, FObjectDiffNode, FListDiffNode, FDictionaryDiffNode>
    {
        using TVariantWrapper::TVariantWrapper;
    };

    template <>
    struct TIsVariant<FDiffNode> : std::true_type
    {
    };

    JSON_VARIANT_BEGIN(FDiffNode)
        JSON_VARIANT_TYPE(FValueSetNode, TEXT("ValueSet"))
        JSON_VARIANT_TYPE(FValueResetNode, TEXT("ValueReset"))
        JSON_VARIANT_TYPE(FObjectDiffNode, TEXT("Object"))
        JSON_VARIANT_TYPE(FListDiffNode, TEXT("List"))
        JSON_VARIANT_TYPE(FDictionaryDiffNode, TEXT("Dictionary"))
    JSON_VARIANT_END

    template POKESHARPEDITOR_API struct TJsonConverter<FDiffNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FValueSetNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FValueResetNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FObjectDiffNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListDiffNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FDictionaryDiffNode>;
} // namespace PokeEdit
