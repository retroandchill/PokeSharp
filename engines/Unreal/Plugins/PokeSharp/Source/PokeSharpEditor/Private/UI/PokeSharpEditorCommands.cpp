// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/PokeSharpEditorCommands.h"
#include "UI/PokeSharpStyle.h"

#define LOCTEXT_NAMESPACE "FPokeSharpEditorCommands"

FPokeSharpEditorCommands::FPokeSharpEditorCommands()
    : TCommands(TEXT("PokeSharpCommands"),
                NSLOCTEXT("Contexts", "PokeSharpCommands", "PokeSharp"),
                NAME_None,
                FPokeSharpStyle::GetStyleSetName())
{
}

void FPokeSharpEditorCommands::RegisterCommands()
{
    UI_COMMAND(ImportPbsData,
               "Import PBS Data",
               "Import game data from the PBS text files",
               EUserInterfaceActionType::Button,
               FInputChord());
    UI_COMMAND(ExportPbsData,
               "Export PBS Data",
               "Export game data to the PBS text files",
               EUserInterfaceActionType::Button,
               FInputChord());
    UI_COMMAND(EditData,
               "Edit Game Data",
               "Open the PokeSharp data editor to edit game data",
               EUserInterfaceActionType::Button,
               FInputChord());
}

#undef LOCTEXT_NAMESPACE