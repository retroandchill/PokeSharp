// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CommonActivatableWidget.h"
#include "Engine/CancellableAsyncAction.h"
#include "Engine/StreamableManager.h"
#include "GameplayTagContainer.h"

#include "PushWidgetToLayerAsyncAction.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FPushContentToLayerAsyncDelegate, UCommonActivatableWidget *, UserWidget);

/**
 *
 */
UCLASS(MinimalAPI, BlueprintType)
class UPushWidgetToLayerAsyncAction : public UCancellableAsyncAction
{
    GENERATED_BODY()

  public:
    UFUNCTION(BlueprintCallable,
              BlueprintCosmetic,
              BlueprintInternalUseOnly,
              meta = (DefaultToSelf = "OwningPlayer",
                      DeterminesOutputType = "InWidgetClass",
                      DynamicOutputParam = "UserWidget"))
    static POKESHARPCORE_API UPushWidgetToLayerAsyncAction *PushWidgetToLayerAsync(
        APlayerController *OwningPlayer,
        UPARAM(meta = (AllowAbstract = false)) TSoftClassPtr<UCommonActivatableWidget> InWidgetClass,
        UPARAM(meta = (Categories = "UI.Layer")) FGameplayTag InLayerName,
        bool bSuspendInputUntilComplete = true);

    POKESHARPCORE_API void Activate() override;
    POKESHARPCORE_API void Cancel() override;

  private:
    UPROPERTY(BlueprintAssignable)
    FPushContentToLayerAsyncDelegate ConfigureWidget;

    UPROPERTY(BlueprintAssignable)
    FPushContentToLayerAsyncDelegate OnComplete;

    FGameplayTag LayerName;
    bool bSuspendInputUntilComplete = false;
    TWeakObjectPtr<APlayerController> OwningPlayerPtr;
    TSoftClassPtr<UCommonActivatableWidget> WidgetClass;

    TSharedPtr<FStreamableHandle> StreamingHandle;
};