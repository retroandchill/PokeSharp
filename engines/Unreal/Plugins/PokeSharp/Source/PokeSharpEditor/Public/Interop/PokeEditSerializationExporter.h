// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSBindsManager.h"
#include "UObject/Object.h"
#include "PokeEditSerializationExporter.generated.h"

/**
 * 
 */
UCLASS()
class POKESHARPEDITOR_API UPokeEditSerializationExporter : public UObject
{
    GENERATED_BODY()

public:
    UNREALSHARP_FUNCTION()
    static void SerializeString(const TCHAR* Buffer, int32 Length, FString& Output);
    
    UNREALSHARP_FUNCTION()
    static void SerializeByteArray(const uint8* Buffer, int32 Length, TArray<uint8>& Output);

};
