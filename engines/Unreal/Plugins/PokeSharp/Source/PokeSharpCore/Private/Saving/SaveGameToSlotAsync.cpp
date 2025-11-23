// Fill out your copyright notice in the Description page of Project Settings.

#include "Saving/SaveGameToSlotAsync.h"
#include "Kismet/GameplayStatics.h"

void USaveGameToSlotAsync::SaveGameToSlot(const FString &SlotName, const int32 UserIndex, USaveGame *SaveGame)
{
    UGameplayStatics::AsyncSaveGameToSlot(
        SaveGame, SlotName, UserIndex,
        FAsyncSaveGameToSlotDelegate::CreateWeakLambda(this, [this](const FString &, int32, bool bSuccess) {
            bSucceeded = bSuccess;
            InvokeManagedCallback();
        }));
}