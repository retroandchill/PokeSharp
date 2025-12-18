// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/DiffNode.h"
#include "PokeEdit/Serialization/JsonSchema.h"

namespace PokeEdit
{
    JSON_OBJECT_SCHEMA_BEGIN(FValueSetNode)
        JSON_FIELD_REQUIRED(NewValue)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FValueSetNode);

    JSON_OBJECT_SCHEMA_BEGIN(FValueResetNode)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FValueResetNode);

    JSON_OBJECT_SCHEMA_BEGIN(FObjectDiffNode)
        JSON_FIELD_REQUIRED(Properties)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FObjectDiffNode);

    JSON_OBJECT_SCHEMA_BEGIN(FListDiffNode)
        JSON_FIELD_REQUIRED(Edits)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FListDiffNode);

    JSON_OBJECT_SCHEMA_BEGIN(FDictionaryDiffNode)
        JSON_FIELD_REQUIRED(Edits)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FDictionaryDiffNode);

    JSON_VARIANT_BEGIN(FDiffNode)
        JSON_VARIANT_TYPE(FValueSetNode, TEXT("ValueSet"))
        JSON_VARIANT_TYPE(FValueResetNode, TEXT("ValueReset"))
        JSON_VARIANT_TYPE(FObjectDiffNode, TEXT("Object"))
        JSON_VARIANT_TYPE(FListDiffNode, TEXT("List"))
        JSON_VARIANT_TYPE(FDictionaryDiffNode, TEXT("Dictionary"))
    JSON_VARIANT_END

    DEFINE_JSON_CONVERTERS(FDiffNode);

    JSON_OBJECT_SCHEMA_BEGIN(FListSetNode)
        JSON_FIELD_REQUIRED(Index)
        JSON_FIELD_REQUIRED(Change)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FListSetNode);

    JSON_OBJECT_SCHEMA_BEGIN(FListAddNode)
        JSON_FIELD_REQUIRED(NewValue)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FListAddNode);

    JSON_OBJECT_SCHEMA_BEGIN(FListInsertNode)
        JSON_FIELD_REQUIRED(Index)
        JSON_FIELD_REQUIRED(NewValue)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FListInsertNode);

    JSON_OBJECT_SCHEMA_BEGIN(FListRemoveNode)
        JSON_FIELD_REQUIRED(Index)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FListRemoveNode);

    JSON_OBJECT_SCHEMA_BEGIN(FListSwapNode)
        JSON_FIELD_REQUIRED(IndexA)
        JSON_FIELD_REQUIRED(IndexB)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FListSwapNode);

    JSON_VARIANT_BEGIN(FListEditNode)
        JSON_VARIANT_TYPE(FListSetNode, TEXT("Set"))
        JSON_VARIANT_TYPE(FListAddNode, TEXT("Add"))
        JSON_VARIANT_TYPE(FListInsertNode, TEXT("Insert"))
        JSON_VARIANT_TYPE(FListRemoveNode, TEXT("Remove"))
        JSON_VARIANT_TYPE(FListSwapNode, TEXT("Swap"))
    JSON_VARIANT_END

    DEFINE_JSON_CONVERTERS(FListEditNode);

    JSON_OBJECT_SCHEMA_BEGIN(FDictionarySetNode)
        JSON_FIELD_REQUIRED(Key)
        JSON_FIELD_REQUIRED(Change)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FDictionarySetNode);

    JSON_OBJECT_SCHEMA_BEGIN(FDictionaryAddNode)
        JSON_FIELD_REQUIRED(Key)
        JSON_FIELD_REQUIRED(Value)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FDictionaryAddNode);

    JSON_OBJECT_SCHEMA_BEGIN(FDictionaryRemoveNode)
        JSON_FIELD_REQUIRED(Key)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FDictionaryRemoveNode);

    JSON_OBJECT_SCHEMA_BEGIN(FDictionaryChangeKeyNode)
        JSON_FIELD_REQUIRED(OldKey)
        JSON_FIELD_REQUIRED(NewKey)
    JSON_OBJECT_SCHEMA_END

    DEFINE_JSON_CONVERTERS(FDictionaryChangeKeyNode);

    JSON_VARIANT_BEGIN(FDictionaryEditNode)
        JSON_VARIANT_TYPE(FDictionarySetNode, TEXT("Set"))
        JSON_VARIANT_TYPE(FDictionaryAddNode, TEXT("Add"))
        JSON_VARIANT_TYPE(FDictionaryRemoveNode, TEXT("Remove"))
        JSON_VARIANT_TYPE(FDictionaryChangeKeyNode, TEXT("ChangeKey"))
    JSON_VARIANT_END

    DEFINE_JSON_CONVERTERS(FDictionaryEditNode);
} // namespace PokeEdit