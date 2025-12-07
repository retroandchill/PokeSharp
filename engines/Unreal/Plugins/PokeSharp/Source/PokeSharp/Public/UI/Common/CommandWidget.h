// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "SelectableWidget.h"

#include "CommandWidget.generated.h"

class UPokemonButtonBase;
class UInputAction;

USTRUCT(BlueprintType, meta = (NullableEnable, RecordStruct, ReadOnly, UseProperties))
struct FCommandData
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite)
    FText Text;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (Nullable))
    TObjectPtr<UInputAction> InputAction;
};

/**
 *
 */
UCLASS(Abstract)
class POKESHARP_API UCommandWidget : public USelectableWidget
{
    GENERATED_BODY()

  public:
    const TArray<FCommandData> &GetCommands() const
    {
        return Commands;
    }

    void SetCommands(TArray<FCommandData> InCommands);

  protected:
    void NativePreConstruct() override;

    UFUNCTION(BlueprintImplementableEvent, Category = "Display")
    void SlotWidget(UPokemonButtonBase *Button, int32 Index);

  private:
    void RefreshCommands();

    UPROPERTY(EditAnywhere, Category = "Selection")
    TSubclassOf<UPokemonButtonBase> ButtonClass;

    UPROPERTY(EditAnywhere,
              BlueprintReadWrite,
              Getter,
              Setter,
              Category = "Selection",
              meta = (AllowPrivateAccess = "true"))
    TArray<FCommandData> Commands;
};
