// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PokeEdit/Serialization/JsonSchemaFwd.h"

#include "Type.generated.h"

namespace PokeEdit
{
    class FJsonStructHandle;
}

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

    static TSharedRef<PokeEdit::FJsonStructHandle> CreateJsonHandle(FName Name, int32 Index);
};

DECLARE_JSON_OBJECT(POKESHARPEDITOR_API, FType);