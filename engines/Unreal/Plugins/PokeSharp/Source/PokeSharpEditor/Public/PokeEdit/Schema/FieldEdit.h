// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "FieldPath.h"
#include <variant>

namespace PokeEdit
{
    struct FSetValueEdit
    {
        FFieldPath Path;
        TSharedRef<FJsonValue> NewValue;

        FSetValueEdit(FFieldPath InPath, const TSharedRef<FJsonValue> &InNewValue)
            : Path(MoveTemp(InPath)), NewValue(InNewValue)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FSetValueEdit)
        JSON_FIELD_REQUIRED(Path)
        JSON_FIELD_REQUIRED(NewValue)
    JSON_OBJECT_SCHEMA_END

    struct FListAddEdit
    {
        FFieldPath Path;
        TSharedRef<FJsonValue> NewItem;

        FListAddEdit(FFieldPath InPath, const TSharedRef<FJsonValue> &InNewItem)
            : Path(MoveTemp(InPath)), NewItem(InNewItem)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListAddEdit)
        JSON_FIELD_REQUIRED(Path)
        JSON_FIELD_REQUIRED(NewItem)
    JSON_OBJECT_SCHEMA_END

    struct FListInsertEdit
    {
        FFieldPath Path;
        int32 Index;
        TSharedRef<FJsonValue> NewItem;

        FListInsertEdit(FFieldPath InPath, const int32 InIndex, const TSharedRef<FJsonValue> &InNewItem)
            : Path(MoveTemp(InPath)), Index(InIndex), NewItem(InNewItem)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListInsertEdit)
        JSON_FIELD_REQUIRED(Path)
        JSON_FIELD_REQUIRED(Index)
        JSON_FIELD_REQUIRED(NewItem)
    JSON_OBJECT_SCHEMA_END

    struct FListRemoveAtEdit
    {
        FFieldPath Path;
        int32 Index;
        TSharedPtr<FJsonValue> OriginalItem;

        FListRemoveAtEdit(FFieldPath InPath, const int32 InIndex) : Path(MoveTemp(InPath)), Index(InIndex)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListRemoveAtEdit)
        JSON_FIELD_REQUIRED(Path)
        JSON_FIELD_REQUIRED(Index)
        JSON_FIELD_OPTIONAL(OriginalItem)
    JSON_OBJECT_SCHEMA_END

    struct FListSwapEdit
    {
        FFieldPath Path;
        int32 IndexA;
        int32 IndexB;

        FListSwapEdit(FFieldPath InPath, const int32 InIndexA, const int32 InIndexB)
            : Path(MoveTemp(InPath)), IndexA(InIndexA), IndexB(InIndexB)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FListSwapEdit)
        JSON_FIELD_REQUIRED(Path)
        JSON_FIELD_REQUIRED(IndexA)
        JSON_FIELD_REQUIRED(IndexB)
    JSON_OBJECT_SCHEMA_END

    struct FDictionarySetEntryEdit
    {
        FFieldPath Path;
        TSharedRef<FJsonValue> Key;
        TSharedRef<FJsonValue> NewValue;

        FDictionarySetEntryEdit(FFieldPath InPath,
                                const TSharedRef<FJsonValue> &Key,
                                const TSharedRef<FJsonValue> &NewValue)
            : Path(MoveTemp(InPath)), Key(Key), NewValue(NewValue)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FDictionarySetEntryEdit)
        JSON_FIELD_REQUIRED(Path)
        JSON_FIELD_REQUIRED(Key)
        JSON_FIELD_REQUIRED(NewValue)
    JSON_OBJECT_SCHEMA_END

    struct FDictionaryRemoveEntryEdit
    {
        FFieldPath Path;
        TSharedRef<FJsonValue> Key;
        TSharedPtr<FJsonValue> OriginalItem;

        FDictionaryRemoveEntryEdit(FFieldPath InPath, const TSharedRef<FJsonValue> &InKey)
            : Path(MoveTemp(InPath)), Key(InKey)
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FDictionaryRemoveEntryEdit)
        JSON_FIELD_REQUIRED(Path)
        JSON_FIELD_REQUIRED(Key)
        JSON_FIELD_OPTIONAL(OriginalItem)
    JSON_OBJECT_SCHEMA_END

    struct FOptionalResetEdit
    {
        FFieldPath Path;
        TSharedPtr<FJsonValue> OriginalItem;

        explicit FOptionalResetEdit(FFieldPath InPath) : Path(MoveTemp(InPath))
        {
        }
    };

    JSON_OBJECT_SCHEMA_BEGIN(FOptionalResetEdit)
        JSON_FIELD_REQUIRED(Path)
        JSON_FIELD_OPTIONAL(OriginalItem)
    JSON_OBJECT_SCHEMA_END

    using FFieldEdit = TVariant<FSetValueEdit,
                                FListAddEdit,
                                FListInsertEdit,
                                FListRemoveAtEdit,
                                FListSwapEdit,
                                FDictionarySetEntryEdit,
                                FDictionaryRemoveEntryEdit,
                                FOptionalResetEdit>;

    template <typename S>
        requires std::same_as<std::decay_t<S>, FFieldEdit>
    decltype(auto) GetPath(S &&Self)
    {
        return Visit([](auto &&Edit) -> decltype(auto) { return std::forward_like<S>(Edit.Path); },
                     std::forward_like<S>(Self));
    }

    JSON_VARIANT_BEGIN(FFieldEdit)
        JSON_VARIANT_TYPE(FSetValueEdit, TEXT("SetValue"))
        JSON_VARIANT_TYPE(FListAddEdit, TEXT("ListAdd"))
        JSON_VARIANT_TYPE(FListInsertEdit, TEXT("ListInsert"))
        JSON_VARIANT_TYPE(FListRemoveAtEdit, TEXT("ListRemoveAt"))
        JSON_VARIANT_TYPE(FListSwapEdit, TEXT("ListSwap"))
        JSON_VARIANT_TYPE(FDictionarySetEntryEdit, TEXT("DictionarySetEntry"))
        JSON_VARIANT_TYPE(FDictionaryRemoveEntryEdit, TEXT("DictionaryRemoveEntry"))
        JSON_VARIANT_TYPE(FOptionalResetEdit, TEXT("OptionalReset"))
    JSON_VARIANT_END

    template POKESHARPEDITOR_API struct TJsonConverter<FSetValueEdit>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListAddEdit>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListInsertEdit>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListRemoveAtEdit>;
    template POKESHARPEDITOR_API struct TJsonConverter<FListSwapEdit>;
    template POKESHARPEDITOR_API struct TJsonConverter<FDictionarySetEntryEdit>;
    template POKESHARPEDITOR_API struct TJsonConverter<FDictionaryRemoveEntryEdit>;
    template POKESHARPEDITOR_API struct TJsonConverter<FOptionalResetEdit>;
    template POKESHARPEDITOR_API struct TJsonConverter<FFieldEdit>;
} // namespace PokeEdit
