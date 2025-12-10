// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PokeEdit/Serialization/JsonMacros.h"

#include "Type.generated.h"

/**
 *
 */
USTRUCT(BlueprintType)
struct POKESHARPEDITOR_API FType
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    FName Id;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    FText Name;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data", meta = (UIMin = 0, ClampMin = 0))
    int32 IconPosition;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    bool IsSpecialType;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    bool IsPseudoType;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    TArray<FName> Weaknesses;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    TArray<FName> Resistances;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    TArray<FName> Immunities;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    TArray<FName> Flags;
};

JSON_OBJECT_SCHEMA_BEGIN(FType)
    JSON_FIELD_OPTIONAL(Id)
    JSON_FIELD_OPTIONAL(Name)
    JSON_FIELD_OPTIONAL(IconPosition)
    JSON_FIELD_OPTIONAL(IsSpecialType)
    JSON_FIELD_OPTIONAL(IsPseudoType)
    JSON_FIELD_OPTIONAL(Weaknesses)
    JSON_FIELD_OPTIONAL(Resistances)
    JSON_FIELD_OPTIONAL(Immunities)
    JSON_FIELD_OPTIONAL(Flags)
JSON_OBJECT_SCHEMA_END

template struct POKESHARPEDITOR_API PokeEdit::TJsonConverter<FType>;