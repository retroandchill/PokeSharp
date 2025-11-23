// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSBindsManager.h"
#include "UObject/Object.h"

#include "FileHandleExporter.generated.h"

enum class ESeekOrigin : int32
{
    Begin = 0,
    Current = 1,
    End = 2
};

/**
 *
 */
UCLASS()
class POKESHARPCORE_API UFileHandleExporter : public UObject
{
    GENERATED_BODY()

  public:
    UNREALSHARP_FUNCTION()
    static int64 GetSize(IFileHandle *Handle);

    UNREALSHARP_FUNCTION()
    static int64 GetPosition(IFileHandle *Handle);

    UNREALSHARP_FUNCTION()
    static bool Flush(IFileHandle *Handle);

    UNREALSHARP_FUNCTION()
    static bool Read(IFileHandle *Handle, uint8 *Buffer, int32 Size, int32 &OutRead);

    UNREALSHARP_FUNCTION()
    static bool Seek(IFileHandle *Handle, uint64 Offset, ESeekOrigin Origin, int64 &NewPosition);

    UNREALSHARP_FUNCTION()
    static bool SetLength(IFileHandle *Handle, uint64 NewLength);

    UNREALSHARP_FUNCTION()
    static bool Write(IFileHandle *Handle, const uint8 *Buffer, int32 Size);

    UNREALSHARP_FUNCTION()
    static void Close(const IFileHandle *Handle);
};
