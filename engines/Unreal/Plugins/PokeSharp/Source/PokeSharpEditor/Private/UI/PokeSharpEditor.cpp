// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/PokeSharpEditor.h"
#include "PokeEdit/PokeEditApi.h"
#include "SlateOptMacros.h"

BEGIN_SLATE_FUNCTION_BUILD_OPTIMIZATION

void SPokeSharpEditor::Construct(const FArguments &InArgs)
{
    if (auto EditorTabs = PokeEdit::GetEditorTabs(); EditorTabs.HasError())
    {
        // clang-format off
        ChildSlot
        [
            SNew(SBorder)
            .Padding(8.0f)
            [
                SNew(STextBlock)
                    .Text(FText::FromString(EditorTabs.StealError()))
            ]
        ];
        // clang-format on
    }
    else
    {
        // clang-format off
        ChildSlot
        [
            SNew(SBorder)
            .Padding(8.0f)
            [
                SNew(STextBlock)
                    .Text(FText::FromString(TEXT("Successfully got the tabs")))
            ]
        ];
        // clang-format on
    }
}

END_SLATE_FUNCTION_BUILD_OPTIMIZATION