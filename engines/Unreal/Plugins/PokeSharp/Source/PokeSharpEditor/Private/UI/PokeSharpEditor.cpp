// Fill out your copyright notice in the Description page of Project Settings.


#include "UI/PokeSharpEditor.h"

#include "SlateOptMacros.h"

BEGIN_SLATE_FUNCTION_BUILD_OPTIMIZATION

void SPokeSharpEditor::Construct(const FArguments &InArgs)
{
    // clang-format off
    ChildSlot
    [
        SNew(SBorder)
        .Padding(8.0f)
        [
            SNew(STextBlock)
            .Text(FText::FromString(TEXT("PokeSharp Data Editor - TODO: build UI here")))
        ]
    ];
    // clang-format on
}

END_SLATE_FUNCTION_BUILD_OPTIMIZATION