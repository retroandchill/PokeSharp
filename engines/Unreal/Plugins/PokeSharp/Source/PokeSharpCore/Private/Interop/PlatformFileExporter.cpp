// Fill out your copyright notice in the Description page of Project Settings.

#include "Interop/PlatformFileExporter.h"

IFileHandle *UPlatformFileExporter::OpenRead(const TCHAR *FileName, const bool bAllowWrite)
{
    auto &PlatformFile = FPlatformFileManager::Get().GetPlatformFile();

    if (FString AbsolutePath; FPackageName::TryConvertLongPackageNameToFilename(FString(FileName), AbsolutePath))
    {
        return PlatformFile.OpenRead(AbsolutePath.GetCharArray().GetData(), bAllowWrite);
    }

    return nullptr;
}

IFileHandle *UPlatformFileExporter::OpenWrite(const TCHAR *FileName, const bool bAppend, const bool bAllowRead,
                                              const bool bOverwrite)
{
    auto &PlatformFile = FPlatformFileManager::Get().GetPlatformFile();
    if (!bOverwrite && PlatformFile.FileExists(FileName))
    {
        return nullptr;
    }

    if (FString AbsolutePath; FPackageName::TryConvertLongPackageNameToFilename(FString(FileName), AbsolutePath))
    {
        return PlatformFile.OpenWrite(FileName, bAppend, bAllowRead);
    }
    return nullptr;
}