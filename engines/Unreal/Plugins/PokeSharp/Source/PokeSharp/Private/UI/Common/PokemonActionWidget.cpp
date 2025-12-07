// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/Common/PokemonActionWidget.h"
#include "Blueprint/UserWidget.h"
#include "CommonInputBaseTypes.h"
#include "CommonInputSubsystem.h"
#include "EnhancedInputSubsystems.h"
#include "OptionalPtr.h"

FSlateBrush UPokemonActionWidget::GetIcon() const
{
    // If there is an Enhanced Input action associated with this widget, then search for any
    // keys bound to that action and display those instead of the default data table settings.
    // This covers the case of when a player has rebound a key to something else
    if (AssociatedInputAction != nullptr)
    {
        if (const auto *EnhancedInputSubsystem = GetEnhancedInputSubsystem(); EnhancedInputSubsystem != nullptr)
        {
            TArray<FKey> BoundKeys = EnhancedInputSubsystem->QueryKeysMappedToAction(AssociatedInputAction);

            const auto *CommonInputSubsystem = GetInputSubsystem();
            if (FSlateBrush SlateBrush;
                !BoundKeys.IsEmpty() && CommonInputSubsystem &&
                UCommonInputPlatformSettings::Get()->TryGetInputBrush(SlateBrush,
                                                                      BoundKeys[0],
                                                                      CommonInputSubsystem->GetCurrentInputType(),
                                                                      CommonInputSubsystem->GetCurrentGamepadName()))
            {
                return SlateBrush;
            }
        }
    }

    return Super::GetIcon();
}

UEnhancedInputLocalPlayerSubsystem *UPokemonActionWidget::GetEnhancedInputSubsystem() const
{
    return TOptionalPtr(DisplayedBindingHandle.GetBoundWidget())
        .Map([](const UWidget *Widget) { return Widget->GetOwningLocalPlayer(); })
        .OrElseGet([this] { return GetOwningLocalPlayer(); })
        .Map([](const ULocalPlayer *Player) { return Player->GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(); })
        .Get();
}