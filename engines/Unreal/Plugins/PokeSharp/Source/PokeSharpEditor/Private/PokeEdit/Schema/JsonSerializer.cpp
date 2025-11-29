// Fill out your copyright notice in the Description page of Project Settings.


#include "PokeEdit/Schema/JsonSerializer.h"

namespace PokeEdit
{
    FString WriteAsString(const TSharedRef<FJsonValue>& Value)
    {
        FString Result;
        const auto Writer = TJsonWriterFactory<>::Create(&Result);
        FJsonSerializer::Serialize(Value, FString(), Writer);
        return Result;
    }
}