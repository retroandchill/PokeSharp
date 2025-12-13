// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/DiffNode.h"

namespace PokeEdit
{
    template struct TJsonConverter<FDiffNode>;
    template struct TJsonConverter<FValueSetNode>;
    template struct TJsonConverter<FValueResetNode>;
    template struct TJsonConverter<FObjectDiffNode>;
    template struct TJsonConverter<FListDiffNode>;
    template struct TJsonConverter<FDictionaryDiffNode>;
    
    template struct TJsonConverter<FListEditNode>;
    template struct TJsonConverter<FListSetNode>;
    template struct TJsonConverter<FListAddNode>;
    template struct TJsonConverter<FListInsertNode>;
    template struct TJsonConverter<FListRemoveNode>;
    template struct TJsonConverter<FListSwapNode>;
    
    template struct TJsonConverter<FDictionaryEditNode>;
    template struct TJsonConverter<FDictionarySetNode>;
    template struct TJsonConverter<FDictionaryAddNode>;
    template struct TJsonConverter<FDictionaryRemoveNode>;
    template struct TJsonConverter<FDictionaryChangeKeyNode>;
} // namespace PokeEdit