// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CommonActivatableWidget.h"

#include "SelectableWidget.generated.h"

class UCommonButtonBase;
class UCommonButtonGroupBase;

DECLARE_MULTICAST_DELEGATE_OneParam(FOnButtonAction, int32);

/**
 *
 */
UCLASS(Abstract)
class POKESHARP_API USelectableWidget : public UCommonActivatableWidget
{
    GENERATED_BODY()

  public:
    USelectableWidget();

    int32 GetDesiredFocusIndex() const
    {
        return DesiredFocusIndex;
    }

    void SetDesiredFocusIndex(int32 Index);

    FDelegateHandle BindToOnDesiredFocusIndexChanged(FOnButtonAction::FDelegate Delegate)
    {
        return OnDesiredFocusIndexChanged.Add(MoveTemp(Delegate));
    }

    bool UnbindFromOnDesiredFocusIndexChanged(const FDelegateHandle Handle)
    {
        return OnDesiredFocusIndexChanged.Remove(Handle);
    }

    FDelegateHandle BindToOnButtonClicked(FOnButtonAction::FDelegate &&Delegate)
    {
        return OnButtonClicked.Add(MoveTemp(Delegate));
    }

    bool UnbindFromOnButtonClicked(const FDelegateHandle Handle)
    {
        return OnButtonClicked.Remove(Handle);
    }

    FDelegateHandle BindToOnCancel(FSimpleMulticastDelegate::FDelegate Delegate)
    {
        return OnCancel.Add(MoveTemp(Delegate));
    }

    bool UnbindFromOnCancel(const FDelegateHandle Handle)
    {
        return OnCancel.Remove(Handle);
    }

  protected:
    void NativePreConstruct() override;

    UWidget *NativeGetDesiredFocusTarget() const override;

    bool NativeOnHandleBackAction() override;

    UCommonButtonGroupBase *GetButtons() const
    {
        return Buttons.Get();
    }

  private:
    UPROPERTY(Transient)
    TObjectPtr<UCommonButtonGroupBase> Buttons;

    UPROPERTY(EditAnywhere,
              BlueprintReadWrite,
              Getter,
              Setter,
              Category = "Selection",
              meta = (AllowPrivateAccess = "true"))
    int32 DesiredFocusIndex;

    FOnButtonAction OnDesiredFocusIndexChanged;
    FOnButtonAction OnButtonClicked;
    FSimpleMulticastDelegate OnCancel;
};
