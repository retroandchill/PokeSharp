// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/Components/GameDataEntrySelector.h"
#include "SlateOptMacros.h"
#include "Widgets/Input/SSearchBox.h"

void SGameDataEntrySelector::Construct(const FArguments &InArgs)
{
    OnEntrySelected = InArgs._OnEntrySelected;
    OnGetEntries = InArgs._OnGetEntries;

    // clang-format off
    ChildSlot
    [
        SNew(SVerticalBox)
            // Search bar
            + SVerticalBox::Slot()
                .AutoHeight()
                .Padding(2)
                [
                    SAssignNew(SearchBox, SSearchBox)
                        .OnTextChanged(this, &SGameDataEntrySelector::OnSearchTextChanged)
                        .HintText(NSLOCTEXT("GameDataRepositoryEditor", "SearchBoxHint", "Search entries..."))
                ]

            // Entries list
            + SVerticalBox::Slot()
                .FillHeight(1.f)
                [
                    SAssignNew(EntriesList, SListView<TSharedPtr<FEntryRowData>>)
                        .ListItemsSource(&FilteredEntries)
                        .OnGenerateRow(this, &SGameDataEntrySelector::OnGenerateRow)
                        .OnSelectionChanged(this, &SGameDataEntrySelector::OnSelectionChanged)
                        .SelectionMode(ESelectionMode::Single)
                ]
    ];
    // clang-format on

    RefreshList();
}

void SGameDataEntrySelector::RefreshList()
{
    if (OnGetEntries.IsBound())
    {
        AllEntries = OnGetEntries.Execute();
        FilteredEntries = AllEntries;
        EntriesList->RequestListRefresh();
    }
}

void SGameDataEntrySelector::SelectAtIndex(const int32 Index)
{
    if (Index >= 0 && Index < AllEntries.Num())
    {
        EntriesList->SetSelection(AllEntries[Index]);
    }
}

TArray<TSharedPtr<FEntryRowData>> SGameDataEntrySelector::GetSelectedEntries() const
{
    return EntriesList->GetSelectedItems();
}

bool SGameDataEntrySelector::IsFiltering() const
{
    return FilteredEntries.Num() < AllEntries.Num();
}

// ReSharper disable once CppPassValueParameterByConstReference
TSharedRef<ITableRow> SGameDataEntrySelector::OnGenerateRow(TSharedPtr<FEntryRowData> Item,
                                                            const TSharedRef<STableViewBase> &OwnerTable) const
{
    const int32 TotalDigits = FMath::Max(AllEntries.Num() / 10 + 1, 1);
    FNumberFormattingOptions NumberOptions;
    NumberOptions.MinimumIntegralDigits = TotalDigits;

    // clang-format off
    return SNew(STableRow<TSharedPtr<FEntryRowData>>, OwnerTable)
        [
            SNew(SHorizontalBox)
                // Index column
                + SHorizontalBox::Slot()
                    .AutoWidth()
                    .Padding(5)
                    [
                        SNew(STextBlock)
                            .Text(FText::AsNumber(Item->Index + 1, &NumberOptions))
                    ]

                // Name column
                + SHorizontalBox::Slot()
                    .FillWidth(1.0f)
                    .Padding(5)
                    [
                        SNew(STextBlock)
                            .Text(Item->Label)
                    ]
        ];
    // clang-format on
}

void SGameDataEntrySelector::OnSearchTextChanged(const FText &InSearchText)
{
    FilteredEntries.Empty();
    const FString SearchString = InSearchText.ToString();

    for (const auto &Entry : AllEntries)
    {
        if (SearchString.IsEmpty() || Entry->Label.ToString().Contains(SearchString))
        {
            FilteredEntries.Add(Entry);
        }
    }

    EntriesList->RequestListRefresh();
}

// ReSharper disable once CppPassValueParameterByConstReference
void SGameDataEntrySelector::OnSelectionChanged(TSharedPtr<FEntryRowData> Item, ESelectInfo::Type) const
{
    if (OnEntrySelected.IsBound())
    {
        OnEntrySelected.Execute(Item);
    }
}