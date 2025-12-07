// Fill out your copyright notice in the Description page of Project Settings.

#include "Interop/PokeEditCallbacks.h"

FPokeEditManager &FPokeEditManager::Get()
{
    static FPokeEditManager Instance;
    return Instance;
}

void FPokeEditManager::SetCallbacks(const FPokeEditCallbacks NewCallbacks)
{
    Callbacks = NewCallbacks;
}

std::expected<void, FString> FPokeEditManager::SendRequest(const FName RequestName,
                                                           const TSharedRef<FArchive> &Payload,
                                                           const TSharedRef<FArchive> &Response) const
{
    FString Error;
    if (Callbacks.SendRequest(RequestName, Payload, Response, Error))
    {
        return {};
    }

    return std::unexpected(MoveTemp(Error));
}