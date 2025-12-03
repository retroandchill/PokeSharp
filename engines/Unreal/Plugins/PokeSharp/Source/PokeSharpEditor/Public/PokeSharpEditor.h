#pragma once

#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"

class FPokeSharpEditorModule : public IModuleInterface
{
  public:
    void StartupModule() override;
    void ShutdownModule() override;

  private:
    void RegisterCommands();
    void RegisterMenu() const;
    TSharedRef<SWidget> GeneratePokeSharpMenu() const;

    static void ImportPbsData();
    static void ExportPbsData();
    static void EditData();

    TSharedPtr<FUICommandList> PokeSharpCommands;
};
