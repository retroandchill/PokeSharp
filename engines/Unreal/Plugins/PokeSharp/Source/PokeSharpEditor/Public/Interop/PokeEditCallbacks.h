// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

/**
 *
 */
struct FPokeEditCallbacks
{
    using FSendRequest = bool(__stdcall *)(FName, const TSharedRef<FArchive> &, const TSharedRef<FArchive> &,
                                           FString &);

    FSendRequest SendRequest = nullptr;
};

class FPokeEditManager
{
    FPokeEditManager() = default;
    ~FPokeEditManager() = default;

  public:
    static FPokeEditManager &Get();

    void SetCallbacks(FPokeEditCallbacks NewCallbacks);

    TValueOrError<void, FString> SendRequest(FName RequestName, const TSharedRef<FArchive> &Payload,
                                             const TSharedRef<FArchive> &Response) const;

  private:
    FPokeEditCallbacks Callbacks;
};
