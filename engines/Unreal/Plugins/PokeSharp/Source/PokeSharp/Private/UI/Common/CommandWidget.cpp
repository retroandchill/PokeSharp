// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/Common/CommandWidget.h"
#include "Blueprint/WidgetTree.h"
#include "Groups/CommonButtonGroupBase.h"
#include "UI/Common/PokemonButtonBase.h"

void UCommandWidget::SetCommands(TArray<FCommandData> InCommands)
{
    Commands = MoveTemp(InCommands);
    RefreshCommands();
}

void UCommandWidget::RefreshCommands()
{
    for (int32 i = 0; i < GetButtons()->GetButtonCount(); i++)
    {
        auto *Button = GetButtons()->GetButtonBaseAtIndex(i);
        Button->RemoveFromParent();
    }

    GetButtons()->RemoveAll();
    for (auto &[Text, Action] : Commands)
    {
        auto *NewButton = WidgetTree->ConstructWidget(ButtonClass);
        NewButton->SetButtonText(Text);
        if (Action != nullptr)
        {
            NewButton->SetTriggeringEnhancedInputAction(Action);
        }

        SlotWidget(NewButton, GetButtons()->GetButtonCount());
        GetButtons()->AddWidget(NewButton);
    }
}