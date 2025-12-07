// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PokeEdit/Schema/FieldPath.h"
#include "Widgets/SCompoundWidget.h"

class SBorder;
/**
 *
 */
class POKESHARPEDITOR_API SGameDataEntryDetails : public SCompoundWidget
{
  public:
    SLATE_BEGIN_ARGS(SGameDataEntryDetails)
        {
        }

    SLATE_END_ARGS()

    /** Constructs this widget with InArgs */
    void Construct(const FArguments &InArgs);

    void SetEntryPath(PokeEdit::FFieldPath InPath);
    void ClearEntryPath();

  private:
    void RefreshContent();

    TSharedPtr<SBorder> Contents;

    TOptional<PokeEdit::FFieldPath> EntryPath;
};
