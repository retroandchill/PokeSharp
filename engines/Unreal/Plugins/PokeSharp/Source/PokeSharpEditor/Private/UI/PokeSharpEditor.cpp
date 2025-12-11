// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/PokeSharpEditor.h"
#include "Components/VerticalBox.h"
#include "Framework/Commands/UICommandList.h"
#include "Framework/MultiBox/MultiBoxBuilder.h"
#include "LogPokeSharpEditor.h"
#include "PokeEdit/PokeEditApi.h"
#include "Styling/AppStyle.h"
#include "UI/Components/DefaultEditorPage.h"
#include "Widgets/Input/SButton.h"
#include "Widgets/Layout/SBorder.h"
#include "Widgets/Layout/SSeparator.h"
#include "Widgets/Layout/SUniformWrapPanel.h"
#include "Widgets/Text/STextBlock.h"

void SPokeSharpEditor::Construct(const FArguments &InArgs, const TSharedRef<SDockTab> &InOwner)
{
    Owner = InOwner;
    GetStructForTabDelegate = InArgs._GetStructForTab;
    // clang-format off
    ChildSlot
    [
        SNew(SVerticalBox)

            // Toolbar at the very top, with the same background as asset editor toolbars
            + SVerticalBox::Slot()
                .AutoHeight()
                [
                    SAssignNew(ToolbarContainer, SBorder)
                    .BorderImage(FAppStyle::Get().GetBrush("AssetEditorToolbar.Background"))
                    .Padding(FMargin(4.0f, 2.0f))
                ]

            // Thin separator under the toolbar
            + SVerticalBox::Slot()
                .AutoHeight()
                [
                    SNew(SSeparator)
                ]

            // Tab bar row
            +SVerticalBox::Slot()
                .AutoHeight()
                [
                    SAssignNew(TabBar, SUniformWrapPanel)
                ]

            // Separator between tabs and content
            + SVerticalBox::Slot()
                .AutoHeight()
                [
                    SNew(SSeparator)
                ]
            
            // Main content area (active tab content)
            + SVerticalBox::Slot()
                .FillHeight(1.f)
                [
                    SAssignNew(ContentArea, SBorder)
                    .Padding(4.0f)
                ]
    ];
    // clang-format on

    RebuildToolbar();
    RefreshTabs();
}

void SPokeSharpEditor::RefreshTabs()
{
    auto EditorTabs = PokeEdit::GetEditorTabs();
    if (!EditorTabs.has_value())
    {
        UE_LOG(LogPokeSharpEditor, Error, TEXT("Error fetching tabs: %s"), *EditorTabs.error())
        return;
    }

    auto &Tabs = EditorTabs.value();
    if (!Tabs.IsEmpty())
    {
        CurrentTab = Tabs[0].Id;
    }

    for (const auto &[Id, Name] : Tabs)
    {
        // clang-format off
        TabBar->AddSlot()
            [
                SNew(SButton)
                    .IsEnabled_Lambda([this, Id] { return CurrentTab != Id; })
                    .OnClicked_Lambda([this, Id]
                    {
                        CurrentTab = Id;
                        RebuildCurrentTabContent();
                        return FReply::Handled();
                    })
                    [
                        SNew(STextBlock).Text(Name)
                    ]
            ];
        // clang-format on
    }

    RebuildCurrentTabContent();
}

// ReSharper disable once CppMemberFunctionMayBeConst
void SPokeSharpEditor::RebuildCurrentTabContent()
{
    if (!ensure(!CurrentTab.IsNone()))
    {
        ContentArea->SetContent(SNullWidget::NullWidget);
        return;
    }

    // In the future: ask C# what kind of page this tab wants (dockable vs simple).
    // For now: always build the default data editor page.
    ContentArea->SetContent(SNew(SDefaultEditorPage,
                                 Owner.Pin().ToSharedRef(),
                                 CurrentTab,
                                 GetStructForTabDelegate.Execute(CurrentTab).ToSharedRef()));
}

// ReSharper disable once CppMemberFunctionMayBeConst
void SPokeSharpEditor::RebuildToolbar()
{
    // In a full editor, you’d keep this as a member and bind real commands.
    const auto CommandList = MakeShared<FUICommandList>();

    FToolBarBuilder ToolbarBuilder(CommandList, FMultiBoxCustomization::None);
    ToolbarBuilder.SetStyle(&FAppStyle::Get(), "AssetEditorToolbar");

    // Example: basic section with Save / Undo / Redo
    ToolbarBuilder.BeginSection("Main");
    ToolbarBuilder.AddToolBarButton(FUIAction(FExecuteAction::CreateLambda(
                                        [this]
                                        {
                                            // TODO: call C# view-model save for CurrentTab
                                        })),
                                    NAME_None,
                                    FText::FromString(TEXT("Save")),
                                    FText::FromString(TEXT("Save current asset")),
                                    FSlateIcon(FAppStyle::Get().GetStyleSetName(), "Icons.Save"));

    ToolbarBuilder.AddToolBarButton(FUIAction(FExecuteAction::CreateLambda(
                                        [this]
                                        {
                                            // TODO: Undo for CurrentTab
                                        })),
                                    NAME_None,
                                    FText::FromString(TEXT("Undo")),
                                    FText::FromString(TEXT("Undo last action")),
                                    FSlateIcon(FAppStyle::Get().GetStyleSetName(), "GenericCommands.Undo"));

    ToolbarBuilder.AddToolBarButton(FUIAction(FExecuteAction::CreateLambda(
                                        [this]
                                        {
                                            // TODO: Redo for CurrentTab
                                        })),
                                    NAME_None,
                                    FText::FromString(TEXT("Redo")),
                                    FText::FromString(TEXT("Redo last action")),
                                    FSlateIcon(FAppStyle::Get().GetStyleSetName(), "GenericCommands.Redo"));
    ToolbarBuilder.EndSection();

    // Plug the finished toolbar widget into the styled border
    ToolbarContainer->SetContent(ToolbarBuilder.MakeWidget());
}