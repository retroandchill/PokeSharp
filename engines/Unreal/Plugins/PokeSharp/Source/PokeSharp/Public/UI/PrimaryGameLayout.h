// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CommonActivatableWidget.h"
#include "CommonUIExtensions.h"
#include "CommonUserWidget.h"
#include "Engine/AssetManager.h"
#include "Engine/StreamableManager.h"
#include "GameplayTagContainer.h"
#include "Widgets/CommonActivatableWidgetContainer.h"

#include "PrimaryGameLayout.generated.h"

struct FGameplayTag;
struct FStreamableHandle;
class UCommonActivatableWidget;

enum class EAsyncWidgetLayerState : uint8
{
    Canceled,
    Initialize,
    AfterPush
};

/**
 *
 */
UCLASS(MinimalAPI, Abstract, meta = (DisableNativeTick))
class UPrimaryGameLayout : public UCommonUserWidget
{
    GENERATED_BODY()

  public:
    UPrimaryGameLayout() = default;

    POKESHARP_API static UPrimaryGameLayout *GetInstance(const UObject *WorldContextObject);
    POKESHARP_API static UPrimaryGameLayout *GetInstance(const APlayerController *PlayerController);
    POKESHARP_API static UPrimaryGameLayout *GetInstance(ULocalPlayer *LocalPlayer);

    bool IsDormant() const
    {
        return bIsDormant;
    }
    POKESHARP_API void SetIsDormant(bool bNewIsDormant);

    template <std::derived_from<UCommonActivatableWidget> ActivatableWidgetT = UCommonActivatableWidget>
    TSharedPtr<FStreamableHandle> PushWidgetToLayerStackAsync(
        FGameplayTag LayerName, bool bSuspendInputUntilComplete,
        TSoftClassPtr<UCommonActivatableWidget> ActivatableWidgetClass)
    {
        return PushWidgetToLayerStackAsync<ActivatableWidgetT>(LayerName, bSuspendInputUntilComplete,
                                                               ActivatableWidgetClass,
                                                               [](EAsyncWidgetLayerState, ActivatableWidgetT *) {});
    }

    template <std::derived_from<UCommonActivatableWidget> ActivatableWidgetT = UCommonActivatableWidget>
    TSharedPtr<FStreamableHandle> PushWidgetToLayerStackAsync(
        FGameplayTag LayerName, const bool bSuspendInputUntilComplete,
        TSoftClassPtr<UCommonActivatableWidget> ActivatableWidgetClass,
        std::invocable<EAsyncWidgetLayerState, ActivatableWidgetT *> auto StateFunc)
    {
        static FName PushingWidgetToLayer("PushingWidgetToLayer");
        const FName SuspendInputToken =
            bSuspendInputUntilComplete
                ? UCommonUIExtensions::SuspendInputForPlayer(GetOwningPlayer(), PushingWidgetToLayer)
                : NAME_None;

        auto &StreamableManager = UAssetManager::Get().GetStreamableManager();
        TSharedPtr<FStreamableHandle> StreamingHandle = StreamableManager.RequestAsyncLoad(
            ActivatableWidgetClass.ToSoftObjectPath(),
            FStreamableDelegate::CreateWeakLambda(
                this, [this, LayerName, ActivatableWidgetClass, StateFunc, SuspendInputToken] {
                    UCommonUIExtensions::ResumeInputForPlayer(GetOwningPlayer(), SuspendInputToken);

                    ActivatableWidgetT *Widget = PushWidgetToLayerStack<ActivatableWidgetT>(
                        LayerName, ActivatableWidgetClass.Get(), [StateFunc](ActivatableWidgetT &WidgetToInit) {
                            StateFunc(EAsyncWidgetLayerState::Initialize, &WidgetToInit);
                        });

                    StateFunc(EAsyncWidgetLayerState::AfterPush, Widget);
                }));

        // Setup a cancel delegate so that we can resume input if this handler is canceled.
        StreamingHandle->BindCancelDelegate(
            FStreamableDelegate::CreateWeakLambda(this, [this, StateFunc, SuspendInputToken]() {
                UCommonUIExtensions::ResumeInputForPlayer(GetOwningPlayer(), SuspendInputToken);
                StateFunc(EAsyncWidgetLayerState::Canceled, nullptr);
            }));

        return StreamingHandle;
    }

    template <std::derived_from<UCommonActivatableWidget> ActivatableWidgetT = UCommonActivatableWidget>
    ActivatableWidgetT *PushWidgetToLayerStack(FGameplayTag LayerName,
                                               TSubclassOf<ActivatableWidgetT> ActivatableWidgetClass)
    {
        return PushWidgetToLayerStack<ActivatableWidgetT>(LayerName, ActivatableWidgetClass,
                                                          [](ActivatableWidgetT &) {});
    }

    template <std::derived_from<UCommonActivatableWidget> ActivatableWidgetT = UCommonActivatableWidget,
              std::invocable<ActivatableWidgetT &> InitFuncT>
    ActivatableWidgetT *PushWidgetToLayerStack(const FGameplayTag LayerName,
                                               TSubclassOf<ActivatableWidgetT> ActivatableWidgetClass,
                                               InitFuncT &&InitInstanceFunc)
    {
        auto Layer = GetLayerWidget(LayerName);
        return Layer != nullptr
                   ? Layer->AddWidget<ActivatableWidgetT>(ActivatableWidgetClass, Forward<InitFuncT>(InitInstanceFunc))
                   : nullptr;
    }

    POKESHARP_API void FindAndRemoveWidgetFromLayer(UCommonActivatableWidget *WidgetToRemove);

    POKESHARP_API UCommonActivatableWidgetContainerBase *GetLayerWidget(FGameplayTag LayerName) const;

  protected:
    UFUNCTION(BlueprintCallable, Category = "Layer")
    POKESHARP_API void RegisterLayer(UPARAM(meta = (Categories = "UI.Layer")) FGameplayTag LayerTag,
                                     UCommonActivatableWidgetContainerBase *LayerWidget);

    UFUNCTION(BlueprintNativeEvent, Category = "Layer")
    POKESHARP_API void OnIsDormantChanged();

    POKESHARP_API void OnWidgetStackTransitioning(UCommonActivatableWidgetContainerBase *Widget, bool bIsTransitioning);

  private:
    bool bIsDormant = false;

    TArray<FName> SuspendInputTokens;

    UPROPERTY(Transient, meta = (Categories = "UI.Layer"))
    TMap<FGameplayTag, TObjectPtr<UCommonActivatableWidgetContainerBase>> Layers;
};