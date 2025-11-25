// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CSAsyncActionBase.h"

#include "SelectOptionAsync.generated.h"

class USelectableWidget;
/**
 *
 */
UCLASS(meta = (InternalType))
class POKESHARP_API USelectOptionAsync : public UCSAsyncActionBase
{
    GENERATED_BODY()

  public:
    UFUNCTION(meta = (ScriptMethod))
    void SelectOption(USelectableWidget *SelectionWidget);

  private:
    void OnOptionSelected(int32 Index);
    void OnSelectionCancelled();

    UPROPERTY()
    TObjectPtr<USelectableWidget> Widget;

    UPROPERTY()
    TOptional<int32> SelectedIndex;

    FDelegateHandle SelectedHandle;
    FDelegateHandle CancelledHandle;
};
