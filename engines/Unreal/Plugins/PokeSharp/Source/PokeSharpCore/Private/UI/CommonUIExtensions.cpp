// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/CommonUIExtensions.h"
#include "Blueprint/UserWidget.h"
#include "CommonActivatableWidget.h"
#include "CommonInputSubsystem.h"
#include "OptionalPtr.h"
#include "UI/GameUIManagerSubsystem.h"
#include "UI/GameUIPolicy.h"
#include "UI/PrimaryGameLayout.h"

int32 UCommonUIExtensions::InputSuspensions = 0;

ECommonInputType UCommonUIExtensions::GetOwningPlayerInputType(const UUserWidget *WidgetContextObject)
{
    return TOptionalPtr(WidgetContextObject)
        .Map([](const UUserWidget *Widget) { return Widget->GetOwningLocalPlayer(); })
        .Map(&UCommonInputSubsystem::Get)
        .MapToValue(ECommonInputType::Count, &UCommonInputSubsystem::GetCurrentInputType);
}

static bool IsOwningPlayerInputType(const UUserWidget *WidgetContextObject, ECommonInputType InputType)
{
    return TOptionalPtr(WidgetContextObject)
        .Map([](const UUserWidget *Widget) { return Widget->GetOwningLocalPlayer(); })
        .Map(&UCommonInputSubsystem::Get)
        .MapToValue(false, [InputType](const UCommonInputSubsystem *Subsystem) {
            return Subsystem->GetCurrentInputType() == InputType;
        });
}

bool UCommonUIExtensions::IsOwningPlayerUsingTouch(const UUserWidget *WidgetContextObject)
{
    return IsOwningPlayerInputType(WidgetContextObject, ECommonInputType::Touch);
}

bool UCommonUIExtensions::IsOwningPlayerUsingGamepad(const UUserWidget *WidgetContextObject)
{
    return IsOwningPlayerInputType(WidgetContextObject, ECommonInputType::Gamepad);
}

UCommonActivatableWidget *UCommonUIExtensions::PushContentToLayer(const ULocalPlayer *LocalPlayer,
                                                                  FGameplayTag LayerName,
                                                                  TSubclassOf<UCommonActivatableWidget> WidgetClass)
{
    if (ensure(LocalPlayer != nullptr) || !ensure(WidgetClass != nullptr))
    {
        return nullptr;
    }

    return TOptionalPtr(LocalPlayer->GetGameInstance()->GetSubsystem<UGameUIManagerSubsystem>())
        .Map([](UGameUIManagerSubsystem *Subsystem) { return Subsystem->GetCurrentPolicy(); })
        .Map([LocalPlayer](const UGameUIPolicy *Policy) { return Policy->GetRootLayout(LocalPlayer); })
        .Map([LayerName, WidgetClass](UPrimaryGameLayout *Layout) {
            return Layout->PushWidgetToLayerStack(LayerName, WidgetClass);
        })
        .Get();
}

void UCommonUIExtensions::PushStreamedContentToLayer(const ULocalPlayer *LocalPlayer, FGameplayTag LayerName,
                                                     TSoftClassPtr<UCommonActivatableWidget> WidgetClass)
{
    if (ensure(LocalPlayer != nullptr) || !ensure(WidgetClass != nullptr))
    {
        return;
    }

    TOptionalPtr(LocalPlayer->GetGameInstance()->GetSubsystem<UGameUIManagerSubsystem>())
        .Map([](UGameUIManagerSubsystem *Subsystem) { return Subsystem->GetCurrentPolicy(); })
        .Map([LocalPlayer](const UGameUIPolicy *Policy) { return Policy->GetRootLayout(LocalPlayer); })
        .IfPresent([LayerName, &WidgetClass](UPrimaryGameLayout *Layout) {
            return Layout->PushWidgetToLayerStackAsync(LayerName, true, MoveTemp(WidgetClass));
        });
}

void UCommonUIExtensions::PopContentFromLayer(UCommonActivatableWidget *ActivatableWidget)
{
    if (ActivatableWidget == nullptr)
    {
        // Ignore request to pop an already deleted widget
        return;
    }

    const auto LocalPlayer =
        TOptionalPtr(ActivatableWidget)
            .Map([](const UCommonActivatableWidget *Widget) { return Widget->GetOwningLocalPlayer(); })
            .Get();

    TOptionalPtr(LocalPlayer)
        .Map([](const ULocalPlayer *Player) {
            return Player->GetGameInstance()->GetSubsystem<UGameUIManagerSubsystem>();
        })
        .Map([](const UGameUIManagerSubsystem *Subsystem) { return Subsystem->GetCurrentPolicy(); })
        .Map([LocalPlayer](const UGameUIPolicy *Policy) { return Policy->GetRootLayout(LocalPlayer); })
        .IfPresent([ActivatableWidget](UPrimaryGameLayout *Layout) {
            Layout->FindAndRemoveWidgetFromLayer(ActivatableWidget);
        });
}

ULocalPlayer *UCommonUIExtensions::GetLocalPlayerFromController(APlayerController *PlayerController)
{
    return PlayerController != nullptr ? PlayerController->GetLocalPlayer() : nullptr;
}

FName UCommonUIExtensions::SuspendInputForPlayer(APlayerController *PlayerController, const FName SuspendReason)
{
    return SuspendInputForPlayer(GetLocalPlayerFromController(PlayerController), SuspendReason);
}

FName UCommonUIExtensions::SuspendInputForPlayer(const ULocalPlayer *LocalPlayer, const FName SuspendReason)
{
    const auto CommonInputSubsystem = UCommonInputSubsystem::Get(LocalPlayer);
    if (CommonInputSubsystem == nullptr)
    {
        return NAME_None;
    }

    InputSuspensions++;
    FName SuspendToken = SuspendReason;
    SuspendToken.SetNumber(InputSuspensions);

    CommonInputSubsystem->SetInputTypeFilter(ECommonInputType::MouseAndKeyboard, SuspendToken, true);
    CommonInputSubsystem->SetInputTypeFilter(ECommonInputType::Gamepad, SuspendToken, true);
    CommonInputSubsystem->SetInputTypeFilter(ECommonInputType::Touch, SuspendToken, true);

    return SuspendToken;
}

void UCommonUIExtensions::ResumeInputForPlayer(APlayerController *PlayerController, const FName SuspendToken)
{
    ResumeInputForPlayer(GetLocalPlayerFromController(PlayerController), SuspendToken);
}

void UCommonUIExtensions::ResumeInputForPlayer(const ULocalPlayer *LocalPlayer, const FName SuspendToken)
{
    if (SuspendToken == NAME_None)
    {
        return;
    }

    if (auto *CommonInputSubsystem = UCommonInputSubsystem::Get(LocalPlayer); CommonInputSubsystem != nullptr)
    {
        CommonInputSubsystem->SetInputTypeFilter(ECommonInputType::MouseAndKeyboard, SuspendToken, false);
        CommonInputSubsystem->SetInputTypeFilter(ECommonInputType::Gamepad, SuspendToken, false);
        CommonInputSubsystem->SetInputTypeFilter(ECommonInputType::Touch, SuspendToken, false);
    }
}

UCommonActivatableWidget *UCommonUIExtensions::GetTopWidgetForPlayer(APlayerController *PlayerController,
                                                                     const FGameplayTag LayerName)
{
    return GetTopWidgetForPlayer(GetLocalPlayerFromController(PlayerController), LayerName);
}

UCommonActivatableWidget *UCommonUIExtensions::GetTopWidgetForPlayer(const ULocalPlayer *LocalPlayer,
                                                                     const FGameplayTag LayerName)
{
    const auto *LayerWidget = UPrimaryGameLayout::GetInstance(LocalPlayer)->GetLayerWidget(LayerName);
    return LayerWidget->GetActiveWidget();
}

APlayerController *UCommonUIExtensions::GetPrimaryPlayerController(const UObject *WorldContext)
{
    return WorldContext != nullptr ? WorldContext->GetWorld()->GetFirstPlayerController() : nullptr;
}