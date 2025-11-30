// Fill out your copyright notice in the Description page of Project Settings.

#include "Interop/ArchiveStreamExporter.h"
#include "Interop/FileHandleExporter.h"

void UArchiveStreamExporter::Copy(TSharedRef<FArchive> &Archive, const TSharedRef<FArchive> &OtherArchive)
{
    std::construct_at(&Archive, OtherArchive);
}

void UArchiveStreamExporter::Release(TSharedRef<FArchive> &Archive)
{
    std::destroy_at(&Archive);
}

bool UArchiveStreamExporter::CanRead(const FArchive &Archive)
{
    return Archive.IsLoading();
}

bool UArchiveStreamExporter::CanWrite(const FArchive &Archive)
{
    return Archive.IsSaving();
}

bool UArchiveStreamExporter::CanSeek(FArchive &Archive)
{
    return GetLength(Archive) != INDEX_NONE && GetPosition(Archive) != INDEX_NONE;
}

int64 UArchiveStreamExporter::GetLength(FArchive &Archive)
{
    return Archive.TotalSize();
}

int64 UArchiveStreamExporter::GetPosition(FArchive &Archive)
{
    return Archive.Tell();
}

bool UArchiveStreamExporter::SetPosition(FArchive &Archive, const int64 NewPosition)
{
    if (!CanSeek(Archive))
        return false;
    Archive.Seek(NewPosition);
    return true;
}

void UArchiveStreamExporter::Flush(FArchive &Archive)
{
    Archive.Flush();
}

bool UArchiveStreamExporter::Read(FArchive &Archive, uint8 *Buffer, const int32 Length, int32 &OutRead)
{
    if (!Archive.IsLoading())
        return false;

    const auto Size = Archive.TotalSize();
    const auto Pos = Archive.Tell();

    // If size is unknown, just assume we can read it all
    if (Size == INDEX_NONE || Pos == INDEX_NONE)
    {
        Archive.Serialize(Buffer, Length);
        OutRead = Length;
        return true;
    }

    const auto Remaining = Size - Pos;
    if (Remaining <= 0)
    {
        return 0;
    }

    int32 ToRead = Length;
    if (Remaining < Length)
    {
        ToRead = static_cast<int32>(Remaining);
    }

    Archive.Serialize(Buffer, ToRead);
    OutRead = ToRead;
    return true;
}

bool UArchiveStreamExporter::Seek(FArchive &Archive, const int64 Offset, const ESeekOrigin Origin, int64 &NewPosition)
{
    if (!CanSeek(Archive))
        return false;

    switch (Origin)
    {
    case ESeekOrigin::Begin:
        Archive.Seek(Offset);
        break;
    case ESeekOrigin::Current:
        Archive.Seek(Archive.Tell() + Offset);
        break;
    case ESeekOrigin::End:
        Archive.Seek(Archive.TotalSize() - Offset - 1);
        break;
    }

    NewPosition = Archive.Tell();
    return true;
}

bool UArchiveStreamExporter::Write(FArchive &Archive, uint8 *Buffer, const int32 Size)
{
    if (!Archive.IsSaving())
        return false;
    Archive.Serialize(Buffer, Size);
    return true;
}
