// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSBindsManager.h"
#include "UObject/Object.h"

#include "PlatformFileExporter.generated.h"

/**
 *
 */
UCLASS()
class POKESHARPCORE_API UPlatformFileExporter : public UObject
{
    GENERATED_BODY()

  public:
    UNREALSHARP_FUNCTION()
    static IFileHandle *OpenRead(const TCHAR *FileName, bool bAllowWrite = false);

    UNREALSHARP_FUNCTION()
    static IFileHandle *OpenWrite(const TCHAR *FileName, bool bAppend = false, bool bAllowRead = false,
                                  bool bOverwrite = true);

  private:
    static FString ConvertToContentDirPath(const TCHAR *FileName);
};
