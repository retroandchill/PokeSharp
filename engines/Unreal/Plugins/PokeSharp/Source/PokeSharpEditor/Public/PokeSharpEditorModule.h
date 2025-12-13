#pragma once

#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"

namespace PokeEdit
{
    class FJsonStructHandle;
    struct FFieldPath;
} // namespace PokeEdit

class FUICommandList;
class FSpawnTabArgs;
class SDockTab;
class SWidget;

DECLARE_DELEGATE_RetVal_TwoParams(TSharedRef<PokeEdit::FJsonStructHandle>,
                                 FJsonStructHandleFactory,
                                 FName,
                                 int32)

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

    void RegisterTabSpawner();
    static void UnregisterTabSpawner();

    TSharedRef<SDockTab> SpawnPokeSharpEditorTab(const FSpawnTabArgs &SpawnTabArgs) const;

    static const FName PokeSharpEditorTabName;
    TSharedPtr<FUICommandList> PokeSharpCommands;
    TMap<FName, FJsonStructHandleFactory> ModelMapping;
};
