// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/DictionaryEditNode.h"
#include "PokeEdit/Schema/DiffNode.h"
#include "PokeEdit/Schema/ListEditNode.h"

namespace PokeEdit
{
    template struct TJsonConverter<FListEditNode>;
    template struct TJsonConverter<FListSetNode>;
    template struct TJsonConverter<FListAddNode>;
    template struct TJsonConverter<FListInsertNode>;
    template struct TJsonConverter<FListRemoveNode>;
    template struct TJsonConverter<FListSwapNode>;
} // namespace PokeEdit