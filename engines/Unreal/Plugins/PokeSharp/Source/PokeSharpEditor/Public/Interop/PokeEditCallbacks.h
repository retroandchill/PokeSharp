// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Templates/ValueOrError.h"
#include <expected>

/**
 *
 */
struct FPokeEditCallbacks
{
    using FSendRequest = bool(__stdcall *)(FName,
                                           FName,
                                           const uint8*,
                                           const size_t*,
                                           int32,
                                           uint8*,
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

    std::expected<void, FString> SendRequest(FName ControllerName,
                                             FName MethodName,
                                             const uint8* Payload,
                                             TConstArrayView<size_t> ArgumentOffsets,
                                             uint8* Response) const;

private:
    FPokeEditCallbacks Callbacks;
};
