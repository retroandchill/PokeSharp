// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/Common/PokemonButtonBase.h"
#include "CommonActionWidget.h"

void UPokemonButtonBase::SetButtonText(const FText &InText)
{
    bOverride_ButtonText = InText.IsEmpty();
    ButtonText = InText;
    RefreshButtonText();
}

void UPokemonButtonBase::NativePreConstruct()
{
    Super::NativePreConstruct();

    UpdateButtonStyle();
    RefreshButtonText();
}

void UPokemonButtonBase::UpdateInputActionWidget()
{
    Super::UpdateInputActionWidget();

    UpdateButtonStyle();
    RefreshButtonText();
}

void UPokemonButtonBase::OnInputMethodChanged(const ECommonInputType CurrentInputType)
{
    Super::OnInputMethodChanged(CurrentInputType);

    UpdateButtonStyle();
}

void UPokemonButtonBase::RefreshButtonText()
{
    if (bOverride_ButtonText || ButtonText.IsEmpty())
    {
        if (InputActionWidget != nullptr)
        {
            if (const auto ActionDisplayText = InputActionWidget->GetDisplayText(); !ActionDisplayText.IsEmpty())
            {
                UpdateButtonText(ActionDisplayText);
                return;
            }
        }
    }

    UpdateButtonText(ButtonText);
}