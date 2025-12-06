// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Framework/Commands/Commands.h"

/**
 *
 */
class POKESHARPEDITOR_API FPokeSharpEditorCommands : public TCommands<FPokeSharpEditorCommands>
{
  public:
    FPokeSharpEditorCommands();

    void RegisterCommands() override;

    const TSharedPtr<FUICommandInfo> &GetImportPbsData() const
    {
        return ImportPbsData;
    }
    const TSharedPtr<FUICommandInfo> &GetExportPbsData() const
    {
        return ExportPbsData;
    }
    const TSharedPtr<FUICommandInfo> &GetEditData() const
    {
        return EditData;
    }

  private:
    TSharedPtr<FUICommandInfo> ImportPbsData;
    TSharedPtr<FUICommandInfo> ExportPbsData;
    TSharedPtr<FUICommandInfo> EditData;
};
