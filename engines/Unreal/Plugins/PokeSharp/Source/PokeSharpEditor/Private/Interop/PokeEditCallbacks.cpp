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

std::expected<void, FString> FPokeEditManager::SendRequest(const FName ControllerName,
    const FName MethodName,
    const uint8 *Payload,
    const TConstArrayView<size_t> ArgumentOffsets,
    uint8 *Response) const
{
    FString Error;
    if (Callbacks.SendRequest(ControllerName, MethodName, Payload, ArgumentOffsets.GetData(), ArgumentOffsets.Num(), Response, Error))
    {
        return {};
    }

    return std::unexpected(MoveTemp(Error));
}