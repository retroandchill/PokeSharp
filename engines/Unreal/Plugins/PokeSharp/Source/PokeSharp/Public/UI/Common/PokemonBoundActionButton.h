// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Input/CommonBoundActionButton.h"

#include "PokemonBoundActionButton.generated.h"

/**
 *
 */
UCLASS(MinimalAPI, Abstract, meta = (DisableNativeTick))
class UPokemonBoundActionButton : public UCommonBoundActionButton
{
    GENERATED_BODY()

  protected:
    POKESHARP_API void NativeConstruct() override;

  private:
    void HandleInputMethodChanged(ECommonInputType NewInputMethod);

    UPROPERTY(EditAnywhere, Category = "Styles")
    TSubclassOf<UCommonButtonStyle> KeyboardStyle;

    UPROPERTY(EditAnywhere, Category = "Styles")
    TSubclassOf<UCommonButtonStyle> GamepadStyle;

    UPROPERTY(EditAnywhere, Category = "Styles")
    TSubclassOf<UCommonButtonStyle> TouchStyle;
};
