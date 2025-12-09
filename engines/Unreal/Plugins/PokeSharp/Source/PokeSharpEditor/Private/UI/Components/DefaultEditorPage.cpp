// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/Components/DefaultEditorPage.h"
#include "Framework/Docking/TabManager.h"
#include "IStructureDetailsView.h"
#include "JsonObjectConverter.h"
#include "LogPokeSharpEditor.h"
#include "Mcro/CommonCore.h"
#include "Modules/ModuleManager.h"
#include "PokeEdit/PokeEditApi.h"
#include "PokeEdit/Schema/FieldPath.h"
#include "PropertyEditorModule.h"
#include "Serialization/JsonSerializer.h"
#include "UI/Components/GameDataEntrySelector.h"
#include "Widgets/Docking/SDockTab.h"

void SDefaultEditorPage::Construct(const FArguments &InArgs,
                                   const TSharedRef<SDockTab> &InOuterTab,
                                   const FName InTabId,
                                   const UStruct *InModel)
{
    TabId = InTabId;
    Model = InModel;
    OuterTab = InOuterTab;

    InnerTabManager = FGlobalTabmanager::Get()->NewTabManager(InOuterTab);

    // Register spawners for inner tabs
    InnerTabManager->RegisterTabSpawner("PokeSharp_Entries",
                                        FOnSpawnTab::CreateSP(this, &SDefaultEditorPage::SpawnEntriesTab));

    InnerTabManager->RegisterTabSpawner("PokeSharp_Details",
                                        FOnSpawnTab::CreateSP(this, &SDefaultEditorPage::SpawnDetailsTab));

    // Define the left/right layout (entries / details)
    const auto Layout =
        FTabManager::NewLayout("PokeSharp_InnerLayout_V1")
            ->AddArea(FTabManager::NewPrimaryArea()
                          ->SetOrientation(Orient_Horizontal)
                          ->Split(FTabManager::NewStack()->SetSizeCoefficient(0.3f)->AddTab("PokeSharp_Entries",
                                                                                            ETabState::OpenedTab))
                          ->Split(FTabManager::NewStack()->SetSizeCoefficient(0.7f)->AddTab("PokeSharp_Details",
                                                                                            ETabState::OpenedTab)));

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
                .OnEntrySelected(FOnEntrySelected::CreateSPLambda(this,
                    [this](const TSharedPtr<FEntryRowData> &Entry) 
                    {
                        if (Entry == nullptr)
                        {
                            EntryStruct.Reset();
                            SelectedEntryIndex = INDEX_NONE;
                        }
                        else
                        {
                            using FResult = std::expected<TSharedPtr<FStructOnScope>, FString>;
                        
                            SelectedEntryIndex = Entry->Index;
                            EntryStruct = PokeEdit::GetEntryAtIndex(TabId, SelectedEntryIndex)
                                .transform_error([](FString&& Error) { return FText::FromString(MoveTemp(Error)); })
                                .and_then([this](const TSharedRef<FJsonValue> &EntryJson)
                                {
                                    return PokeEdit::DeserializeFromJson(EntryJson, Model);
                                })
                                .transform([](const TSharedRef<FStructOnScope> &Result)
                                {
                                    return Result.ToSharedPtr();
                                })
                                .or_else([](const FText &Error) -> FResult
                                {
                                    UE_LOG(LogPokeSharpEditor, Error, TEXT("Error fetching entry data: %s"), *Error.ToString());
                                    return TSharedPtr<FStructOnScope>();
                                })
                                .value();
                        }
                        DetailsView->SetStructureData(EntryStruct);
                    }))
        ];
    // clang-format on
}

TSharedRef<SDockTab> SDefaultEditorPage::SpawnDetailsTab(const FSpawnTabArgs &Args)
{
    auto &PropertyEditorModule = FModuleManager::GetModuleChecked<FPropertyEditorModule>("PropertyEditor");
    FDetailsViewArgs DetailsViewArgs;
    DetailsViewArgs.NameAreaSettings = FDetailsViewArgs::HideNameArea;
    const FStructureDetailsViewArgs DetailsViewArgsStruct;

    DetailsView = PropertyEditorModule.CreateStructureDetailView(DetailsViewArgs, DetailsViewArgsStruct, EntryStruct);
    DetailsView->GetOnFinishedChangingPropertiesDelegate().AddLambda(
        [this](const FPropertyChangedEvent &Event)
        {
            // TODO: Actually construct the full information, this is just a basic idea of what we need to do here
            const auto *ValuePtr = Event.Property->ContainerPtrToValuePtr<void>(EntryStruct->GetStructMemory());
            auto Serialized = FJsonObjectConverter::UPropertyToJsonValue(Event.Property, ValuePtr).ToSharedRef();

            PokeEdit::FFieldPath PropertyPath(PokeEdit::FPropertySegment(TabId),
                                              PokeEdit::FListIndexSegment(SelectedEntryIndex),
                                              PokeEdit::FPropertySegment(Event.Property->GetFName()));
        });

    // clang-format off
    return SNew(SDockTab)
        .Label(NSLOCTEXT("SDefaultEditorPage", "DetailsTabLabel", "Details"))
        .TabRole(PanelTab)
        [
            DetailsView->GetWidget().ToSharedRef()
        ];
    // clang-format on
}