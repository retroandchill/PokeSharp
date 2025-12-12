// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "DiffNode.h"

namespace PokeEdit
{
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
        TSharedRef<FJsonValue> NewItem;

        explicit FListAddNode(const TSharedRef<FJsonValue> &NewItem) : NewItem(NewItem)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListAddNode)
        JSON_FIELD_REQUIRED(NewItem)
    JSON_OBJECT_SCHEMA_END

    struct FListInsertNode
    {
        int32 Index;
        TSharedRef<FJsonValue> NewItem;

        FListInsertNode(const int32 Index, const TSharedRef<FJsonValue> &NewItem) : Index(Index), NewItem(NewItem)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListInsertNode)
        JSON_FIELD_REQUIRED(Index)
        JSON_FIELD_REQUIRED(NewItem)
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
} // namespace PokeEdit
