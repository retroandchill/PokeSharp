// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/Components/GameDataEntryDetails.h"
#include "PokeEdit/PokeEditApi.h"
#include "PokeEdit/Schema/FieldDefinition.h"
#include "Widgets/Layout/SBorder.h"
#include "Widgets/Text/STextBlock.h"

void SGameDataEntryDetails::Construct(const FArguments &InArgs)
{
    // clang-format off
    ChildSlot
    [
        SAssignNew(Contents, SBorder)
    ];
    // clang-format on

    RefreshContent();
}

void SGameDataEntryDetails::SetEntryPath(PokeEdit::FFieldPath InPath)
{
    EntryPath.Emplace(MoveTemp(InPath));
    RefreshContent();
}

void SGameDataEntryDetails::ClearEntryPath()
{
    EntryPath.Reset();
    RefreshContent();
}

void SGameDataEntryDetails::RefreshContent()
{
    if (!EntryPath.IsSet())
    {
        Contents->SetContent(SNullWidget::NullWidget);
        return;
    }

    Contents->SetContent(
        PokeEdit::GetFieldDefinition(*EntryPath)
            .transform([](const TSharedRef<PokeEdit::FFieldDefinition> &Definition) -> TSharedRef<SWidget> {
                // clang-format off
            return SNew(STextBlock)
                .Text(FText::FromString(TEXT("Details panel goes here")));
                // clang-format on
            })
            .or_else([](FString &&Error) -> std::expected<TSharedRef<SWidget>, FString> {
                // clang-format off
            return SNew(STextBlock)
                .Text(FText::FromString(MoveTemp(Error)));
                // clang-format on
            })
            .value());
}