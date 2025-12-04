// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Widgets/SCompoundWidget.h"

/**
 *
 */
class POKESHARPEDITOR_API SPokeSharpEditor : public SCompoundWidget
{
  public:
    SLATE_BEGIN_ARGS(SPokeSharpEditor)
        {
        }
    SLATE_END_ARGS()

    /** Constructs this widget with InArgs */
    void Construct(const FArguments &InArgs);

    void RefreshTabs();

  private:
    FName CurrentTab;
    TSharedPtr<SHorizontalBox> TabBar;
};