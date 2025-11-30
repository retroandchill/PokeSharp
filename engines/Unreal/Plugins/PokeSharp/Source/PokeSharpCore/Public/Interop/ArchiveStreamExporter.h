// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSBindsManager.h"
#include "UObject/Object.h"

#include "ArchiveStreamExporter.generated.h"

class FArchiveStream;
enum class ESeekOrigin;

/**
 *
 */
UCLASS()
class POKESHARPCORE_API UArchiveStreamExporter : public UObject
{
    GENERATED_BODY()

  public:
    UNREALSHARP_FUNCTION()
    static void Copy(TSharedRef<FArchive> &Archive, const TSharedRef<FArchive> &OtherArchive);

    UNREALSHARP_FUNCTION()
    static void Release(TSharedRef<FArchive> &Archive);

    UNREALSHARP_FUNCTION()
    static bool CanRead(const FArchive &Archive);

    UNREALSHARP_FUNCTION()
    static bool CanWrite(const FArchive &Archive);

    UNREALSHARP_FUNCTION()
    static bool CanSeek(FArchive &Archive);

    UNREALSHARP_FUNCTION()
    static int64 GetLength(FArchive &Archive);

    UNREALSHARP_FUNCTION()
    static int64 GetPosition(FArchive &Archive);

    UNREALSHARP_FUNCTION()
    static bool SetPosition(FArchive &Archive, int64 NewPosition);

    UNREALSHARP_FUNCTION()
    static void Flush(FArchive &Archive);

    UNREALSHARP_FUNCTION()
    static bool Read(FArchive &Archive, uint8 *Buffer, int32 Length, int32 &OutRead);

    UNREALSHARP_FUNCTION()
    static bool Seek(FArchive &Archive, int64 Offset, ESeekOrigin Origin, int64 &NewPosition);

    UNREALSHARP_FUNCTION()
    static bool Write(FArchive &Archive, uint8 *Buffer, int32 Size);
};
