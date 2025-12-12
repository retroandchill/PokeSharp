// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/DiffNode.h"
#include "PokeEdit/Schema/DictionaryEditNode.h"
#include "PokeEdit/Schema/ListEditNode.h"

namespace PokeEdit
{
    template struct TJsonConverter<FDiffNode>;
    template struct TJsonConverter<FValueSetNode>;
    template struct TJsonConverter<FValueResetNode>;
    template struct TJsonConverter<FObjectDiffNode>;
    template struct TJsonConverter<FListDiffNode>;
    template struct TJsonConverter<FDictionaryDiffNode>;
} // namespace PokeEdit