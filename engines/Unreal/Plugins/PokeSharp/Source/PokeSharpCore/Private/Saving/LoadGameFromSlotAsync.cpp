// Fill out your copyright notice in the Description page of Project Settings.

#include "Saving/LoadGameFromSlotAsync.h"
#include "Kismet/GameplayStatics.h"

void ULoadGameFromSlotAsync::LoadGameFromSlot(const FString &SlotName, const int32 UserIndex)
{
    UGameplayStatics::AsyncLoadGameFromSlot(
        SlotName,
        UserIndex,
        FAsyncLoadGameFromSlotDelegate::CreateWeakLambda(this,
                                                         [this](const FString &, int32, USaveGame *LoadedData)
                                                         {
                                                             SaveGame = LoadedData;
                                                             InvokeManagedCallback();
                                                         }));
}