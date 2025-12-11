// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Widgets/SCompoundWidget.h"

namespace PokeEdit
{
    class FJsonStructHandle;
}

class SUniformWrapPanel;
class SBorder;
class SDockTab;

DECLARE_DELEGATE_RetVal_OneParam(TSharedPtr<PokeEdit::FJsonStructHandle>, FGetStructForTab, FName);

/**
 *
 */
class POKESHARPEDITOR_API SPokeSharpEditor : public SCompoundWidget
{
  public:
    SLATE_BEGIN_ARGS(SPokeSharpEditor)
        {
        }

        SLATE_EVENT(FGetStructForTab, GetStructForTab)

    SLATE_END_ARGS()

    /** Constructs this widget with InArgs */
    void Construct(const FArguments &InArgs, const TSharedRef<SDockTab> &InOwner);

    void RefreshTabs();

  private:
    void RebuildCurrentTabContent();
    void RebuildToolbar();

    FName CurrentTab;

    TWeakPtr<SDockTab> Owner;
    TSharedPtr<SBorder> ToolbarContainer;
    TSharedPtr<SUniformWrapPanel> TabBar;
    TSharedPtr<SBorder> ContentArea;
    FGetStructForTab GetStructForTabDelegate;
};