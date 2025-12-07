// Fill out your copyright notice in the Description page of Project Settings.


#include "PokeEdit/Schema/FieldPath.h"

namespace PokeEdit
{
    template struct TJsonConverter<FPropertySegment>;
    template struct TJsonConverter<FListIndexSegment>;
    template struct TJsonConverter<FDictionaryKeySegment>;
    template struct TJsonConverter<FFieldPathSegment>;
    template struct TJsonConverter<FFieldPath>;
}


