// Fill out your copyright notice in the Description page of Project Settings.

#include "PokeEdit/Schema/FieldEdit.h"

namespace PokeEdit
{
    template struct TJsonConverter<FSetValueEdit>;
    template struct TJsonConverter<FListAddEdit>;
    template struct TJsonConverter<FListInsertEdit>;
    template struct TJsonConverter<FListRemoveAtEdit>;
    template struct TJsonConverter<FListSwapEdit>;
    template struct TJsonConverter<FDictionarySetEntryEdit>;
    template struct TJsonConverter<FDictionaryRemoveEntryEdit>;
    template struct TJsonConverter<FOptionalResetEdit>;
    template struct TJsonConverter<FFieldEdit>;
} // namespace PokeEdit