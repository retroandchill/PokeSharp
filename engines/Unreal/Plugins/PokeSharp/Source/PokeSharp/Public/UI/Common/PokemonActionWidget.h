// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CommonActionWidget.h"

#include "PokemonActionWidget.generated.h"

class UEnhancedInputLocalPlayerSubsystem;

/**
 *
 */
UCLASS(BlueprintType, Blueprintable)
class POKESHARP_API UPokemonActionWidget : public UCommonActionWidget
{
    GENERATED_BODY()

  public:
    FSlateBrush GetIcon() const override;

    UInputAction *GetAssociatedInputAction() const
    {
        return AssociatedInputAction.Get();
    }

  private:
    UEnhancedInputLocalPlayerSubsystem *GetEnhancedInputSubsystem() const;

    /** The Enhanced Input Action that is associated with this Common Input action. */
    UPROPERTY(BlueprintReadOnly, Getter, EditAnywhere, Category = "Input", meta = (AllowPrivateAccess = "true"))
    const TObjectPtr<UInputAction> AssociatedInputAction;
};
