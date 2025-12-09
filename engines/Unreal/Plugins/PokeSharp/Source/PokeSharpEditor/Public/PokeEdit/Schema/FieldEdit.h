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

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FSetValueEdit>,
                            std::make_tuple(TJsonField<&FSetValueEdit::Path>(TEXT("path")),
                                            TJsonField<&FSetValueEdit::NewValue>(TEXT("newValue"))));
    };

    struct FListAddEdit
    {
        FFieldPath Path;
        TSharedRef<FJsonValue> NewItem;

        FListAddEdit(FFieldPath InPath, const TSharedRef<FJsonValue> &InNewItem)
            : Path(MoveTemp(InPath)), NewItem(InNewItem)
        {
        }

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FListAddEdit>,
                            std::make_tuple(TJsonField<&FListAddEdit::Path>(TEXT("path")),
                                            TJsonField<&FListAddEdit::NewItem>(TEXT("newItem"))));
    };

    struct FListInsertEdit
    {
        FFieldPath Path;
        int32 Index;
        TSharedRef<FJsonValue> NewItem;

        FListInsertEdit(FFieldPath InPath, const int32 InIndex, const TSharedRef<FJsonValue> &InNewItem)
            : Path(MoveTemp(InPath)), Index(InIndex), NewItem(InNewItem)
        {
        }

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FListInsertEdit>,
                            std::make_tuple(TJsonField<&FListInsertEdit::Path>(TEXT("path")),
                                            TJsonField<&FListInsertEdit::Index>(TEXT("index")),
                                            TJsonField<&FListInsertEdit::NewItem>(TEXT("newItem"))));
    };

    struct FListRemoveAtEdit
    {
        FFieldPath Path;
        int32 Index;
        TSharedPtr<FJsonValue> OriginalItem;

        FListRemoveAtEdit(FFieldPath InPath, const int32 InIndex) : Path(MoveTemp(InPath)), Index(InIndex)
        {
        }

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FListRemoveAtEdit>,
                            std::make_tuple(TJsonField<&FListRemoveAtEdit::Path>(TEXT("path")),
                                            TJsonField<&FListRemoveAtEdit::Index>(TEXT("index"))),
                            std::make_tuple(TJsonField<&FListRemoveAtEdit::OriginalItem>(TEXT("originalItem"))));
    };

    struct FListSwapEdit
    {
        FFieldPath Path;
        int32 IndexA;
        int32 IndexB;

        FListSwapEdit(FFieldPath InPath, const int32 InIndexA, const int32 InIndexB)
            : Path(MoveTemp(InPath)), IndexA(InIndexA), IndexB(InIndexB)
        {
        }

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FListSwapEdit>,
                            std::make_tuple(TJsonField<&FListSwapEdit::Path>(TEXT("path")),
                                            TJsonField<&FListSwapEdit::IndexA>(TEXT("indexA")),
                                            TJsonField<&FListSwapEdit::IndexB>(TEXT("indexB"))));
    };

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

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FDictionarySetEntryEdit>,
                            std::make_tuple(TJsonField<&FDictionarySetEntryEdit::Path>(TEXT("path")),
                                            TJsonField<&FDictionarySetEntryEdit::Key>(TEXT("key")),
                                            TJsonField<&FDictionarySetEntryEdit::NewValue>(TEXT("newValue"))));
    };

    struct FDictionaryRemoveEntryEdit
    {
        FFieldPath Path;
        TSharedRef<FJsonValue> Key;
        TSharedPtr<FJsonValue> OriginalItem;

        FDictionaryRemoveEntryEdit(FFieldPath InPath, const TSharedRef<FJsonValue> &InKey)
            : Path(MoveTemp(InPath)), Key(InKey)
        {
        }

        static constexpr auto JsonSchema = TJsonObjectType(
            std::in_place_type<FDictionaryRemoveEntryEdit>,
            std::make_tuple(TJsonField<&FDictionaryRemoveEntryEdit::Path>(TEXT("path")),
                            TJsonField<&FDictionaryRemoveEntryEdit::Key>(TEXT("key"))),
            std::make_tuple(TJsonField<&FDictionaryRemoveEntryEdit::OriginalItem>(TEXT("originalItem"))));
    };

    struct FOptionalResetEdit
    {
        FFieldPath Path;
        TSharedPtr<FJsonValue> OriginalItem;

        explicit FOptionalResetEdit(FFieldPath InPath) : Path(MoveTemp(InPath))
        {
        }

        static constexpr auto JsonSchema =
            TJsonObjectType(std::in_place_type<FOptionalResetEdit>,
                            std::make_tuple(TJsonField<&FOptionalResetEdit::Path>(TEXT("path"))),
                            std::make_tuple(TJsonField<&FOptionalResetEdit::OriginalItem>(TEXT("originalItem"))));
    };

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

    template <>
    struct TJsonUnionTraits<FFieldEdit>
    {
        static constexpr auto JsonSchema = TJsonUnionType(
            TJsonDiscriminator<&FFieldEdit::GetIndex>(),
            TJsonUnionKey<FSetValueEdit, FFieldEdit::IndexOfType<FSetValueEdit>()>(TEXT("SetValue")),
            TJsonUnionKey<FListAddEdit, FFieldEdit::IndexOfType<FListAddEdit>()>(TEXT("ListAdd")),
            TJsonUnionKey<FListInsertEdit, FFieldEdit::IndexOfType<FListInsertEdit>()>(TEXT("ListInsert")),
            TJsonUnionKey<FListRemoveAtEdit, FFieldEdit::IndexOfType<FListRemoveAtEdit>()>(TEXT("ListRemoveAt")),
            TJsonUnionKey<FListSwapEdit, FFieldEdit::IndexOfType<FListSwapEdit>()>(TEXT("ListSwap")),
            TJsonUnionKey<FDictionarySetEntryEdit, FFieldEdit::IndexOfType<FDictionarySetEntryEdit>()>(
                TEXT("DictionarySetEntry")),
            TJsonUnionKey<FDictionaryRemoveEntryEdit, FFieldEdit::IndexOfType<FDictionaryRemoveEntryEdit>()>(
                TEXT("DictionaryRemoveEntry")),
            TJsonUnionKey<FOptionalResetEdit, FFieldEdit::IndexOfType<FOptionalResetEdit>()>(TEXT("OptionalReset")));
    };

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
