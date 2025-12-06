// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Widgets/SCompoundWidget.h"

class SUniformWrapPanel;
class SBorder;
class SDockTab;
/**
 *
 */
class POKESHARPEDITOR_API SPokeSharpEditor : public SCompoundWidget
{
  public:
    SLATE_BEGIN_ARGS(SPokeSharpEditor)
        {
        }

        SLATE_ARGUMENT(TWeakPtr<SDockTab>, Owner)

    SLATE_END_ARGS()

    /** Constructs this widget with InArgs */
    void Construct(const FArguments &InArgs);

    void RefreshTabs();

  private:
    void RebuildCurrentTabContent();
    void RebuildToolbar();

    FName CurrentTab;

    TWeakPtr<SDockTab> Owner;
    TSharedPtr<SBorder> ToolbarContainer;
    TSharedPtr<SUniformWrapPanel> TabBar;
    TSharedPtr<SBorder> ContentArea;
};