// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSBindsManager.h"
#include "UObject/Object.h"

#include "PokeSharpNameExporter.generated.h"

/**
 *
 */
UCLASS()
class POKESHARPCORE_API UPokeSharpNameExporter : public UObject
{
    GENERATED_BODY()

  public:
    UNREALSHARP_FUNCTION()
    static void GetOrAddEntry(const UTF16CHAR *Str,
                              int32 Length,
                              bool FindMode,
                              uint32 &ComparisonIndex,
                              uint32 &DisplayIndex,
                              int32 &Number);

    UNREALSHARP_FUNCTION()
    static bool IsValid(uint32 ComparisonIndex, uint32 DisplayIndex);

    UNREALSHARP_FUNCTION()
    static bool EqualsBuffer(uint32 ComparisonIndex,
                             uint32 DisplayIndex,
                             int32 Number,
                             const UTF16CHAR *Str,
                             int32 Length);

    UNREALSHARP_FUNCTION()
    static void GetString(uint32 ComparisonIndex, uint32 DisplayIndex, int32 Number, FString &OutString);

    UNREALSHARP_FUNCTION()
    static void FromUnrealName(FName Name, uint32 &ComparisonIndex, uint32 &DisplayIndex, int32 &Number);

  private:
    static FName GetName(uint32 ComparisonIndex, uint32 DisplayIndex, int32 Number);
};
