// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/PokeSharpEditor.h"
#include "LogPokeSharpEditor.h"
#include "PokeEdit/PokeEditApi.h"
#include "SlateOptMacros.h"

BEGIN_SLATE_FUNCTION_BUILD_OPTIMIZATION

void SPokeSharpEditor::Construct(const FArguments &InArgs)
{
    // clang-format off
    ChildSlot
    [
        SNew(SVerticalBox)
            +SVerticalBox::Slot()
                .AutoHeight()
                [
                    SAssignNew(TabBar, SHorizontalBox)
                ]
            + SVerticalBox::Slot()
                .AutoHeight()
                [
                    SNew(SSeparator)
                ]
    ];
    // clang-format on

    RefreshTabs();
}

void SPokeSharpEditor::RefreshTabs()
{
    auto EditorTabs = PokeEdit::GetEditorTabs();
    if (const auto *Error = EditorTabs.TryGetError(); Error != nullptr)
    {
        UE_LOG(LogPokeSharpEditor, Error, TEXT("Error fetching tabs: %s"), **Error)
        return;
    }

    auto &Tabs = EditorTabs.GetValue();
    if (!Tabs.IsEmpty())
    {
        CurrentTab = Tabs[0].Id;
    }

    for (const auto &[Id, Name] : Tabs)
    {
        // clang-format off
        TabBar->AddSlot()
            .AutoWidth()
            [
                SNew(SButton)
                    .IsEnabled_Lambda([this, Id] { return CurrentTab != Id; })
                    .OnClicked_Lambda([this, Id]
                    {
                        CurrentTab = Id;
                        return FReply::Handled();
                    })
                    [
                        SNew(STextBlock).Text(Name)
                    ]
            ];
        // clang-format on
    }
}

END_SLATE_FUNCTION_BUILD_OPTIMIZATION