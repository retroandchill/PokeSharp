// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Dom/JsonValue.h"
#include "PokeEdit/Serialization/JsonSchemaFwd.h"
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

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FValueSetNode);

    struct FValueResetNode
    {
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FValueResetNode);

    struct FObjectDiffNode
    {
        TMap<FName, TSharedRef<FDiffNode>> Properties;

        explicit FObjectDiffNode(TMap<FName, TSharedRef<FDiffNode>> Properties) : Properties(MoveTemp(Properties))
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FObjectDiffNode);

    struct FListDiffNode
    {
        TArray<FListEditNode> Edits;

        explicit FListDiffNode(TArray<FListEditNode> Edits) : Edits(MoveTemp(Edits))
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FListDiffNode);

    struct FDictionaryDiffNode
    {
        TArray<FDictionaryEditNode> Edits;

        explicit FDictionaryDiffNode(TArray<FDictionaryEditNode> Edits) : Edits(MoveTemp(Edits))
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FDictionaryDiffNode);

    struct FDiffNode
        : TVariantWrapper<FValueSetNode, FValueResetNode, FObjectDiffNode, FListDiffNode, FDictionaryDiffNode>
    {
        using TVariantWrapper::TVariantWrapper;
    };

    DECLARE_JSON_UNION(POKESHARPEDITOR_API, FDiffNode);

    template <>
    struct TIsVariant<FDiffNode> : std::true_type
    {
    };

    struct FListSetNode
    {
        int32 Index;
        FDiffNode Change;

        explicit FListSetNode(const int32 Index, FDiffNode Change) : Index(Index), Change(MoveTemp(Change))
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FListSetNode);

    struct FListAddNode
    {
        TSharedRef<FJsonValue> NewValue;

        explicit FListAddNode(const TSharedRef<FJsonValue> &NewItem) : NewValue(NewItem)
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FListAddNode);

    struct FListInsertNode
    {
        int32 Index;
        TSharedRef<FJsonValue> NewValue;

        FListInsertNode(const int32 Index, const TSharedRef<FJsonValue> &NewItem) : Index(Index), NewValue(NewItem)
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FListInsertNode);

    struct FListRemoveNode
    {
        int32 Index;

        explicit FListRemoveNode(const int32 Index) : Index(Index)
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FListRemoveNode);

    struct FListSwapNode
    {
        int32 IndexA;
        int32 IndexB;

        FListSwapNode(const int32 IndexA, const int32 IndexB) : IndexA(IndexA), IndexB(IndexB)
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FListSwapNode);

    struct FListEditNode : TVariantWrapper<FListSetNode, FListAddNode, FListInsertNode, FListRemoveNode, FListSwapNode>
    {
        using TVariantWrapper::TVariantWrapper;
    };

    DECLARE_JSON_UNION(POKESHARPEDITOR_API, FListEditNode);

    template <>
    struct TIsVariant<FListEditNode> : std::true_type
    {
    };

    struct FDictionarySetNode
    {
        TSharedRef<FJsonValue> Key;
        FDiffNode Change;

        FDictionarySetNode(const TSharedRef<FJsonValue> &Key, FDiffNode Change) : Key(Key), Change(MoveTemp(Change))
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FDictionarySetNode);

    struct FDictionaryAddNode
    {
        TSharedRef<FJsonValue> Key;
        TSharedRef<FJsonValue> Value;

        FDictionaryAddNode(const TSharedRef<FJsonValue> &Key, const TSharedRef<FJsonValue> &NewItem)
            : Key(Key), Value(NewItem)
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FDictionaryAddNode);

    struct FDictionaryRemoveNode
    {
        TSharedRef<FJsonValue> Key;

        explicit FDictionaryRemoveNode(const TSharedRef<FJsonValue> &Key) : Key(Key)
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FDictionaryRemoveNode);

    struct FDictionaryChangeKeyNode
    {
        TSharedRef<FJsonValue> OldKey;
        TSharedRef<FJsonValue> NewKey;

        FDictionaryChangeKeyNode(const TSharedRef<FJsonValue> &OldKey, const TSharedRef<FJsonValue> &NewKey)
            : OldKey(OldKey), NewKey(NewKey)
        {
        }
    };

    DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FDictionaryChangeKeyNode);

    struct FDictionaryEditNode
        : TVariantWrapper<FDictionarySetNode, FDictionaryAddNode, FDictionaryRemoveNode, FDictionaryChangeKeyNode>
    {
        using TVariantWrapper::TVariantWrapper;
    };

    DECLARE_JSON_UNION(POKESHARPEDITOR_API, FDictionaryEditNode);

    template <>
    struct TIsVariant<FDictionaryEditNode> : std::true_type
    {
    };
} // namespace PokeEdit
