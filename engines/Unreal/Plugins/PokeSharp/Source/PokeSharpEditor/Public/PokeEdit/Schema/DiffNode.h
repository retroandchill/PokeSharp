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
        TMap<FName, TSharedRef<FDiffNode>> Properties;

        explicit FObjectDiffNode(TMap<FName, TSharedRef<FDiffNode>> Properties) : Properties(MoveTemp(Properties))
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FObjectDiffNode)
        JSON_FIELD_REQUIRED(Properties)
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
    
    struct FListSetNode
    {
        int32 Index;
        FDiffNode Change;

        explicit FListSetNode(const int32 Index, FDiffNode Change) : Index(Index), Change(MoveTemp(Change))
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListSetNode)
        JSON_FIELD_REQUIRED(Index)
        JSON_FIELD_REQUIRED(Change)
    JSON_OBJECT_SCHEMA_END

    struct FListAddNode
    {
        TSharedRef<FJsonValue> NewValue;

        explicit FListAddNode(const TSharedRef<FJsonValue> &NewItem) : NewValue(NewItem)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListAddNode)
        JSON_FIELD_REQUIRED(NewValue)
    JSON_OBJECT_SCHEMA_END

    struct FListInsertNode
    {
        int32 Index;
        TSharedRef<FJsonValue> NewValue;

        FListInsertNode(const int32 Index, const TSharedRef<FJsonValue> &NewItem) : Index(Index), NewValue(NewItem)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListInsertNode)
        JSON_FIELD_REQUIRED(Index)
        JSON_FIELD_REQUIRED(NewValue)
    JSON_OBJECT_SCHEMA_END

    struct FListRemoveNode
    {
        int32 Index;

        explicit FListRemoveNode(const int32 Index) : Index(Index)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListRemoveNode)
        JSON_FIELD_REQUIRED(Index)
    JSON_OBJECT_SCHEMA_END

    struct FListSwapNode
    {
        int32 IndexA;
        int32 IndexB;

        FListSwapNode(const int32 IndexA, const int32 IndexB) : IndexA(IndexA), IndexB(IndexB)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListSwapNode)
        JSON_FIELD_REQUIRED(IndexA)
        JSON_FIELD_REQUIRED(IndexB)
    JSON_OBJECT_SCHEMA_END

    struct FListEditNode : TVariantWrapper<FListSetNode, FListAddNode, FListInsertNode, FListRemoveNode, FListSwapNode>
    {
        using TVariantWrapper::TVariantWrapper;
    };

    template <>
    struct TIsVariant<FListEditNode> : std::true_type
    {
    };

    JSON_VARIANT_BEGIN(FListEditNode)
        JSON_VARIANT_TYPE(FListSetNode, TEXT("Set"))
        JSON_VARIANT_TYPE(FListAddNode, TEXT("Add"))
        JSON_VARIANT_TYPE(FListInsertNode, TEXT("Insert"))
        JSON_VARIANT_TYPE(FListRemoveNode, TEXT("Remove"))
        JSON_VARIANT_TYPE(FListSwapNode, TEXT("Swap"))
    JSON_VARIANT_END

    template POKESHARPEDITOR_API struct TJsonConverter<FListEditNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListSetNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListAddNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListInsertNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListRemoveNode>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListSwapNode>;
    
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
        TSharedRef<FJsonValue> Value;

        FDictionaryAddNode(const TSharedRef<FJsonValue> &Key, const TSharedRef<FJsonValue> &NewItem)
            : Key(Key), Value(NewItem)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FDictionaryAddNode)
        JSON_FIELD_REQUIRED(Key)
        JSON_FIELD_REQUIRED(Value)
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
