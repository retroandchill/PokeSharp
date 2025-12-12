// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "DiffNode.h"

namespace PokeEdit
{
    struct FDictionarySetNode
    {
        TSharedRef<FJsonValue> Key;
        FDiffNode Change;

        FDictionarySetNode(const TSharedRef<FJsonValue> &Key, FDiffNode Change) : Key(Key), Change(MoveTemp(Change))
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FDictionarySetNode)
        JSON_FIELD_REQUIRED(Key)
        JSON_FIELD_REQUIRED(Change)
    JSON_OBJECT_SCHEMA_END

    struct FDictionaryAddNode
    {
        TSharedRef<FJsonValue> Key;
        TSharedRef<FJsonValue> NewItem;

        FDictionaryAddNode(const TSharedRef<FJsonValue> &Key, const TSharedRef<FJsonValue> &NewItem)
            : Key(Key), NewItem(NewItem)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FDictionaryAddNode)
        JSON_FIELD_REQUIRED(Key)
        JSON_FIELD_REQUIRED(NewItem)
    JSON_OBJECT_SCHEMA_END

    struct FDictionaryRemoveNode
    {
        TSharedRef<FJsonValue> Key;

        explicit FDictionaryRemoveNode(const TSharedRef<FJsonValue> &Key) : Key(Key)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FDictionaryRemoveNode)
        JSON_FIELD_REQUIRED(Key)
    JSON_OBJECT_SCHEMA_END

    struct FDictionaryChangeKeyNode
    {
        TSharedRef<FJsonValue> OldKey;
        TSharedRef<FJsonValue> NewKey;

        FDictionaryChangeKeyNode(const TSharedRef<FJsonValue> &OldKey, const TSharedRef<FJsonValue> &NewKey)
            : OldKey(OldKey), NewKey(NewKey)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FDictionaryChangeKeyNode)
        JSON_FIELD_REQUIRED(OldKey)
        JSON_FIELD_REQUIRED(NewKey)
    JSON_OBJECT_SCHEMA_END

    struct FDictionaryEditNode
        : TVariantWrapper<FDictionarySetNode, FDictionaryAddNode, FDictionaryRemoveNode, FDictionaryChangeKeyNode>
    {
        using TVariantWrapper::TVariantWrapper;
    };

    template <>
    struct TIsVariant<FDictionaryEditNode> : std::true_type
    {
    };

    JSON_VARIANT_BEGIN(FDictionaryEditNode)
        JSON_VARIANT_TYPE(FDictionarySetNode, TEXT("Set"))
        JSON_VARIANT_TYPE(FDictionaryAddNode, TEXT("Add"))
        JSON_VARIANT_TYPE(FDictionaryRemoveNode, TEXT("Remove"))
        JSON_VARIANT_TYPE(FDictionaryChangeKeyNode, TEXT("ChangeKey"))
    JSON_VARIANT_END

    template POKESHARPEDITOR_API struct TJsonConverter<FDictionaryEditNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FDictionarySetNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FDictionaryAddNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FDictionaryRemoveNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FDictionaryChangeKeyNode>;
} // namespace PokeEdit
