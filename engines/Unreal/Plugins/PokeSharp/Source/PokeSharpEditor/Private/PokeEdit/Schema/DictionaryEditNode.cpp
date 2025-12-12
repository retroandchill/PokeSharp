// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/DictionaryEditNode.h"
#include "PokeEdit/Schema/DiffNode.h"
#include "PokeEdit/Schema/ListEditNode.h"

namespace PokeEdit
{
    template struct TJsonConverter<FDictionaryEditNode>;
    template struct TJsonConverter<FDictionarySetNode>;
    template struct TJsonConverter<FDictionaryAddNode>;
    template struct TJsonConverter<FDictionaryRemoveNode>;
    template struct TJsonConverter<FDictionaryChangeKeyNode>;
} // namespace PokeEdit