// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

/**
 *
 */
class POKESHARPEDITOR_API FPokeEditClient
{
  public:
    TValueOrError<TSharedRef<FJsonValue>, FString> SendRequest(FName RequestName,
                                                               const TSharedRef<FJsonValue> &Payload);
};
