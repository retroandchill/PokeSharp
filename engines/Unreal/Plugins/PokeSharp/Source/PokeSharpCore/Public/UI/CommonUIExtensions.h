// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CommonActivatableWidget.h"
#include "CommonInputTypeEnum.h"
#include "GameplayTagContainer.h"
#include "Kismet/BlueprintFunctionLibrary.h"

#include "CommonUIExtensions.generated.h"

class ULocalPlayer;

/**
 *
 */
UCLASS(MinimalAPI)
class UCommonUIExtensions : public UBlueprintFunctionLibrary
{
    GENERATED_BODY()

  public:
    UFUNCTION(BlueprintPure, BlueprintCosmetic, Category = "Global UI Extensions",
              meta = (WorldContext = "WidgetContextObject", ExtensionMethod))
    static POKESHARPCORE_API ECommonInputType GetOwningPlayerInputType(const UUserWidget *WidgetContextObject);

    UFUNCTION(BlueprintPure, BlueprintCosmetic, Category = "Global UI Extensions",
              meta = (WorldContext = "WidgetContextObject", ExtensionMethod))
    static POKESHARPCORE_API bool IsOwningPlayerUsingTouch(const UUserWidget *WidgetContextObject);

    UFUNCTION(BlueprintPure, BlueprintCosmetic, Category = "Global UI Extensions",
              meta = (WorldContext = "WidgetContextObject", ExtensionMethod))
    static POKESHARPCORE_API bool IsOwningPlayerUsingGamepad(const UUserWidget *WidgetContextObject);

    UFUNCTION(BlueprintCallable, BlueprintCosmetic, Category = "Global UI Extensions",
              meta = (DeterminesOutputType = "WidgetClass", DynamicOutputParam = "ReturnValue", ExtensionMethod))
    static POKESHARPCORE_API UCommonActivatableWidget *PushContentToLayer(
        const ULocalPlayer *LocalPlayer, UPARAM(meta = (Categories = "UI.Layer")) FGameplayTag LayerName,
        UPARAM(meta = (AllowAbstract = false)) TSubclassOf<UCommonActivatableWidget> WidgetClass);

    UFUNCTION(BlueprintCallable, BlueprintCosmetic, Category = "Global UI Extensions", meta = (ExtensionMethod))
    static POKESHARPCORE_API void PushStreamedContentToLayer(
        const ULocalPlayer *LocalPlayer, UPARAM(meta = (Categories = "UI.Layer")) FGameplayTag LayerName,
        UPARAM(meta = (AllowAbstract = false)) TSoftClassPtr<UCommonActivatableWidget> WidgetClass);

    UFUNCTION(BlueprintCallable, BlueprintCosmetic, Category = "Global UI Extensions", meta = (ExtensionMethod))
    static POKESHARPCORE_API void PopContentFromLayer(UCommonActivatableWidget *ActivatableWidget);

    UFUNCTION(BlueprintCallable, BlueprintCosmetic, Category = "Global UI Extensions", meta = (ExtensionMethod))
    static POKESHARPCORE_API ULocalPlayer *GetLocalPlayerFromController(APlayerController *PlayerController);

    UFUNCTION(BlueprintCallable, BlueprintCosmetic, Category = "Global UI Extensions", meta = (ExtensionMethod))
    static POKESHARPCORE_API FName SuspendInputForPlayer(APlayerController *PlayerController, FName SuspendReason);

    static POKESHARPCORE_API FName SuspendInputForPlayer(const ULocalPlayer *LocalPlayer, FName SuspendReason);

    UFUNCTION(BlueprintCallable, BlueprintCosmetic, Category = "Global UI Extensions", meta = (ExtensionMethod))
    static POKESHARPCORE_API void ResumeInputForPlayer(APlayerController *PlayerController, FName SuspendToken);

    static POKESHARPCORE_API void ResumeInputForPlayer(const ULocalPlayer *LocalPlayer, FName SuspendToken);

    UFUNCTION(BlueprintPure, Category = "Global UI Extensions", meta = (ExtensionMethod))
    static POKESHARPCORE_API UCommonActivatableWidget *GetTopWidgetForPlayer(APlayerController *PlayerController,
                                                                             UPARAM(meta = (Categories = "UI.Layer"))
                                                                                 FGameplayTag LayerName);

    static POKESHARPCORE_API UCommonActivatableWidget *GetTopWidgetForPlayer(const ULocalPlayer *LocalPlayer,
                                                                             FGameplayTag LayerName);

    UFUNCTION(BlueprintPure, Category = "Global UI Extensions", meta = (WorldContext = "WorldContext"))
    static POKESHARPCORE_API APlayerController *GetPrimaryPlayerController(const UObject *WorldContext);

  private:
    static int32 InputSuspensions;
};