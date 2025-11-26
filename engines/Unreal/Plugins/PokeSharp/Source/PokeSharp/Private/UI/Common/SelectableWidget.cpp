// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/Common/SelectableWidget.h"
#include "Groups/CommonButtonGroupBase.h"

USelectableWidget::USelectableWidget()
{
    bIsBackHandler = true;
}

void USelectableWidget::SetDesiredFocusIndex(const int32 Index)
{
    if (DesiredFocusIndex == Index)
        return;

    DesiredFocusIndex = FMath::Clamp(Index, INDEX_NONE, Buttons->GetButtonCount() - 1);
    OnDesiredFocusIndexChanged.Broadcast(Index);
}

void USelectableWidget::NativePreConstruct()
{
    Super::NativePreConstruct();

#if WITH_EDITOR
    if (Buttons != nullptr)
        return;
#endif

    Buttons = NewObject<UCommonButtonGroupBase>(this);
    Buttons->NativeOnHoveredButtonBaseChanged.AddWeakLambda(
        this, [this](UCommonButtonBase *, const int32 Index) { SetDesiredFocusIndex(Index); });
    Buttons->NativeOnButtonBaseClicked.AddWeakLambda(this, [this](UCommonButtonBase *, const int32 Index) {
        SetDesiredFocusIndex(Index);
        OnButtonClicked.Broadcast(Index);
    });
}

UWidget *USelectableWidget::NativeGetDesiredFocusTarget() const
{
    if (auto *DesiredTarget = BP_GetDesiredFocusTarget(); DesiredTarget != nullptr)
        return DesiredTarget;

    return Buttons->GetButtonBaseAtIndex(DesiredFocusIndex);
}

bool USelectableWidget::NativeOnHandleBackAction()
{
    if (!bIsBackHandler)
        return false;

    OnCancel.Broadcast();
    return true;
}