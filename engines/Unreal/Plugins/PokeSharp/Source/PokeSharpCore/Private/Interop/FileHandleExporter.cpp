// Fill out your copyright notice in the Description page of Project Settings.

#include "Interop/FileHandleExporter.h"

int64 UFileHandleExporter::GetSize(IFileHandle *Handle)
{
    return Handle->Size();
}

int64 UFileHandleExporter::GetPosition(IFileHandle *Handle)
{
    return Handle->Tell();
}

bool UFileHandleExporter::Flush(IFileHandle *Handle)
{
    return Handle->Flush();
}

bool UFileHandleExporter::Read(IFileHandle *Handle, uint8 *Buffer, const int32 Size, int32 &OutRead)
{
    const auto CurrentPosition = Handle->Tell();
    if (!Handle->Read(Buffer, Size))
    {
        OutRead = 0;
        return false;
    }

    OutRead = static_cast<int32>(Handle->Tell() - CurrentPosition);
    return true;
}

bool UFileHandleExporter::Seek(IFileHandle *Handle, const uint64 Offset, const ESeekOrigin Origin, int64 &NewPosition)
{
    bool Result = false;
    switch (Origin)
    {
    case ESeekOrigin::Begin:
        Result = Handle->Seek(Offset);
        break;
    case ESeekOrigin::Current:
        Result = Handle->Seek(Handle->Tell() + Offset);
        break;
    case ESeekOrigin::End:
        Result = Handle->SeekFromEnd(Offset);
        break;
    }

    if (!Result)
    {
        NewPosition = 0;
        return false;
    }

    NewPosition = Handle->Tell();
    return true;
}

bool UFileHandleExporter::SetLength(IFileHandle *Handle, const uint64 NewLength)
{
    return Handle->Truncate(NewLength);
}

bool UFileHandleExporter::Write(IFileHandle *Handle, const uint8 *Buffer, const int32 Size)
{
    return Handle->Write(Buffer, Size);
}

void UFileHandleExporter::Close(const IFileHandle *Handle)
{
    delete Handle;
}