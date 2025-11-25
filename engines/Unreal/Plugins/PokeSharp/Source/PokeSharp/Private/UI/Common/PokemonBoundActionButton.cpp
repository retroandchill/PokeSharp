// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/Common/PokemonBoundActionButton.h"
#include "CommonInputSubsystem.h"

void UPokemonBoundActionButton::NativeConstruct()
{
    Super::NativeConstruct();

    if (auto *InputSubsystem = GetInputSubsystem(); InputSubsystem != nullptr)
    {
        InputSubsystem->OnInputMethodChangedNative.AddUObject(this, &ThisClass::HandleInputMethodChanged);
        HandleInputMethodChanged(InputSubsystem->GetCurrentInputType());
    }
}

void UPokemonBoundActionButton::HandleInputMethodChanged(const ECommonInputType NewInputMethod)
{
    TSubclassOf<UCommonButtonStyle> NewStyle;
    if (NewInputMethod == ECommonInputType::Gamepad)
    {
        NewStyle = GamepadStyle;
    }
    else if (NewInputMethod == ECommonInputType::Touch)
    {
        NewStyle = TouchStyle;
    }
    else
    {
        NewStyle = KeyboardStyle;
    }

    if (NewStyle != nullptr)
    {
        SetStyle(NewStyle);
    }
}