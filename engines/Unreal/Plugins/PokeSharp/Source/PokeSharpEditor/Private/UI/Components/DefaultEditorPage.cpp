// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/Components/DefaultEditorPage.h"
#include "Framework/Docking/TabManager.h"
#include "LogPokeSharpEditor.h"
#include "Mcro/CommonCore.h"
#include "PokeEdit/PokeEditApi.h"
#include "UI/Components/GameDataEntrySelector.h"
#include "Widgets/Docking/SDockTab.h"

void SDefaultEditorPage::Construct(const FArguments &InArgs)
{
    TabId = InArgs._TabId.Get();
    OuterTab = InArgs._OuterTab;

    InnerTabManager = FGlobalTabmanager::Get()->NewTabManager(OuterTab.Pin().ToSharedRef());

    // Register spawners for inner tabs
    InnerTabManager->RegisterTabSpawner("PokeSharp_Entries",
                                        FOnSpawnTab::CreateSP(this, &SDefaultEditorPage::SpawnEntriesTab));

    InnerTabManager->RegisterTabSpawner("PokeSharp_Details",
                                        FOnSpawnTab::CreateSP(this, &SDefaultEditorPage::SpawnDetailsTab));

    // Define the left/right layout (entries / details)
    const auto Layout = FTabManager::NewLayout("PokeSharp_InnerLayout_V1")
                            ->AddArea(FTabManager::NewPrimaryArea()
                                          ->SetOrientation(Orient_Horizontal)
                                          ->Split(FTabManager::NewStack()->SetSizeCoefficient(0.3f)->AddTab(
                                              "PokeSharp_Entries", ETabState::OpenedTab))
                                          ->Split(FTabManager::NewStack()->SetSizeCoefficient(0.7f)->AddTab(
                                              "PokeSharp_Details", ETabState::OpenedTab)));

    // Restore the layout as a widget and use it as our content
    auto Workspace = InnerTabManager->RestoreFrom(Layout,
                                                  TSharedPtr<SWindow>() // embedded, not its own window
    );

    if (!Workspace.IsValid())
    {
        Workspace = SNew(STextBlock).Text(FText::FromString(TEXT("Failed to restore workspace layout.")));
    }

    ChildSlot[Workspace.ToSharedRef()];
}

TSharedRef<SDockTab> SDefaultEditorPage::SpawnEntriesTab(const FSpawnTabArgs &Args)
{
    using namespace ranges::views;
    using namespace Mcro::Common;

    // clang-format off
    return SNew(SDockTab)
        .Label(NSLOCTEXT("SDefaultEditorPage", "EntriesTabLabel", "Label"))
        .TabRole(PanelTab)
        [
            SNew(SGameDataEntrySelector)
                .OnGetEntries(FOnGetEntries::CreateLambda([TabId = this->TabId]
                {
                    auto Result = PokeEdit::GetEntryLabels(TabId)
                        .transform([](const TArray<FText>& Labels) {
                            return Labels |
                                enumerate |
                                TransformTuple([](int32 Index, const FText& Label) -> TSharedPtr<FEntryRowData> {
                                    return MakeShared<FEntryRowData>(Index, Label);
                                }) |
                                RenderAs<TArray>();
                        });
                    
                    if (Result.has_value())
                    {
                        return MoveTemp(Result).value();
                    }
                    
                    UE_LOG(LogPokeSharpEditor, Error, TEXT("Error fetching entry labels: %s"), *Result.error());
                    return TArray<TSharedPtr<FEntryRowData>>();
                }))
                .OnEntrySelected(FOnEntrySelected::CreateLambda(
                    [TabId = this->TabId](const TSharedPtr<FEntryRowData> &Entry) 
                    {
                                       // TODO: notify C# view-model that Entry was selected for TabId
                    }))
        ];
    // clang-format on
}

TSharedRef<SDockTab> SDefaultEditorPage::SpawnDetailsTab(const FSpawnTabArgs &Args)
{
    // clang-format off
    return SNew(SDockTab)
        .Label(NSLOCTEXT("SDefaultEditorPage", "DetailsTabLabel", "Details"))
        .TabRole(PanelTab)
        [
            // TODO: replace with your real details widget driven by the C# view-model
            SNew(SBorder)
            [
                SNew(STextBlock)
                .Text(FText::FromString(TEXT("Details panel goes here")))
            ]
        ];
    // clang-format on
}