// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

/**
 *
 */
namespace PokeEdit
{
    POKESHARPEDITOR_API TValueOrError<TSharedRef<FJsonValue>, FString> SendRequest(
        FName RequestName, const TSharedRef<FJsonValue> &Payload);
}
