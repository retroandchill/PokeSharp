// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSBindsManager.h"
#include "UObject/Object.h"

#include "PokeSharpTextExporter.generated.h"

/**
 *
 */
UCLASS()
class POKESHARPCORE_API UPokeSharpTextExporter : public UObject
{
    GENERATED_BODY()

  public:
    UNREALSHARP_FUNCTION()
    static void FromSourceString(const UTF16CHAR *Buffer, int32 Length, FText &OutText);

    UNREALSHARP_FUNCTION()
    static void FromLocalized(const UTF16CHAR *NamespaceBuffer, int32 NamespaceLength, const UTF16CHAR *KeyBuffer,
                              int32 KeyLength, const UTF16CHAR *StrBuffer, int32 StrLength, FText &OutText);

    UNREALSHARP_FUNCTION()
    static const FString *GetSourceString(const FText &Text);

    UNREALSHARP_FUNCTION()
    static const FString *GetDisplayString(const FText &Text);

    UNREALSHARP_FUNCTION()
    static bool GetNamespace(const FText &Text, FString &OutNamespace);

    UNREALSHARP_FUNCTION()
    static bool GetKey(const FText &Text, FString &OutKey);

    UNREALSHARP_FUNCTION()
    static bool IsCultureInvariant(const FText &Text);

    UNREALSHARP_FUNCTION()
    static bool IsTransient(const FText &Text);

    UNREALSHARP_FUNCTION()
    static bool ShouldGatherForLocalization(const FText &Text);

    UNREALSHARP_FUNCTION()
    static bool IsEmpty(const FText &Text);

    UNREALSHARP_FUNCTION()
    static bool IsWhitespace(const FText &Text);

    UNREALSHARP_FUNCTION()
    static void Destroy(FText &Text);

    UNREALSHARP_FUNCTION()
    static void AsDisplaySpan(const FText &Text, const TCHAR *&OutBuffer, int32 &OutLength);
};
