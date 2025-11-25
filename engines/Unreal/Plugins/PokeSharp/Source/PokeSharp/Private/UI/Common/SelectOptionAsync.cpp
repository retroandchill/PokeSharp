// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/Common/SelectOptionAsync.h"
#include "UI/Common/SelectableWidget.h"

void USelectOptionAsync::SelectOption(USelectableWidget *SelectionWidget)
{
    Widget = SelectionWidget;
    SelectedHandle = Widget->BindToOnButtonClicked(
        FOnButtonAction::FDelegate::CreateUObject(this, &USelectOptionAsync::OnOptionSelected));
    CancelledHandle =
        Widget->BindToOnCancel(FSimpleDelegate::CreateUObject(this, &USelectOptionAsync::OnSelectionCancelled));
    Widget->ActivateWidget();
}

void USelectOptionAsync::OnOptionSelected(const int32 Index)
{
    SelectedIndex = Index;
    Widget->UnbindFromOnButtonClicked(SelectedHandle);
    Widget->UnbindFromOnCancel(CancelledHandle);
    InvokeManagedCallback();
}

void USelectOptionAsync::OnSelectionCancelled()
{
    SelectedIndex.Reset();
    Widget->UnbindFromOnButtonClicked(SelectedHandle);
    Widget->UnbindFromOnCancel(CancelledHandle);
    InvokeManagedCallback();
}