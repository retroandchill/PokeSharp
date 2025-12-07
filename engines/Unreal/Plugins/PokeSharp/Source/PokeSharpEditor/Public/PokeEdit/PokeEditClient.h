// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Templates/ValueOrError.h"
#include <expected>

class FJsonValue;

/**
 *
 */
namespace PokeEdit
{
    POKESHARPEDITOR_API std::expected<TSharedRef<FJsonValue>, FString> SendRequest(
        FName RequestName, const TSharedRef<FJsonValue> &Payload);
}
