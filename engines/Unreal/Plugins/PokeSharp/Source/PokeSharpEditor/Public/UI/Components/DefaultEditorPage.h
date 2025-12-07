// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Widgets/SCompoundWidget.h"

class SGameDataEntryDetails;
class FTabManager;
class FSpawnTabArgs;
class SDockTab;
/**
 *
 */
class POKESHARPEDITOR_API SDefaultEditorPage : public SCompoundWidget
{
  public:
    SLATE_BEGIN_ARGS(SDefaultEditorPage)
        {
        }

        SLATE_ATTRIBUTE(FName, TabId)

    SLATE_END_ARGS()

    /** Constructs this widget with InArgs */
    void Construct(const FArguments &InArgs, const TSharedRef<SDockTab> &InOuterTab);

  private:
    // tab spawn handlers
    TSharedRef<SDockTab> SpawnEntriesTab(const FSpawnTabArgs &Args);
    TSharedRef<SDockTab> SpawnDetailsTab(const FSpawnTabArgs &Args);

    // inner workspace tab manager (for dockable tabs inside this page)
    TSharedPtr<FTabManager> InnerTabManager;

    // resolved from the attribute in Construct
    FName TabId;
    TWeakPtr<SDockTab> OuterTab;

    TSharedPtr<SGameDataEntryDetails> Details;
};
