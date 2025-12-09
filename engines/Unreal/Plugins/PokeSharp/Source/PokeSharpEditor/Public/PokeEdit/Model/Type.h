// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

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
    uint8 IsSpecialType : 1;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    uint8 IsPseudoType : 1;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    TArray<FName> Weaknesses;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    TArray<FName> Resistances;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    TArray<FName> Immunities;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Data")
    TArray<FName> Flags;
};
