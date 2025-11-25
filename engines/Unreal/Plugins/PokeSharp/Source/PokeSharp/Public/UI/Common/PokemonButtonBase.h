// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CommonButtonBase.h"

#include "PokemonButtonBase.generated.h"

/**
 *
 */
UCLASS(Abstract, BlueprintType, Blueprintable)
class POKESHARP_API UPokemonButtonBase : public UCommonButtonBase
{
    GENERATED_BODY()

  public:
    UFUNCTION(BlueprintCallable)
    void SetButtonText(const FText &InText);

  protected:
    void NativePreConstruct() override;
    void UpdateInputActionWidget() override;
    void OnInputMethodChanged(ECommonInputType CurrentInputType) override;

    void RefreshButtonText();

    UFUNCTION(BlueprintImplementableEvent)
    void UpdateButtonText(const FText &InText);

    UFUNCTION(BlueprintImplementableEvent)
    void UpdateButtonStyle();

  private:
    UPROPERTY(EditAnywhere, Category = "Button", meta = (InlineEditConditionToggle))
    uint8 bOverride_ButtonText : 1;

    UPROPERTY(EditAnywhere, Category = "Button", meta = (EditCondition = "bOverride_ButtonText"))
    FText ButtonText;
};
