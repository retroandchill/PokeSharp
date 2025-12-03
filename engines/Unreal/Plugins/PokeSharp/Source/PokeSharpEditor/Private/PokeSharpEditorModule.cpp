#include "PokeSharpEditorModule.h"
#include "LevelEditor.h"
#include "UI/PokeSharpEditor.h"
#include "UI/PokeSharpEditorCommands.h"
#include "UI/PokeSharpStyle.h"

#define LOCTEXT_NAMESPACE "FPokeSharpEditorModule"

FName FPokeSharpEditorModule::PokeSharpEditorTabName(TEXT("PokeSharpEditorTab"));

void FPokeSharpEditorModule::StartupModule()
{
    FPokeSharpStyle::Initialize();
    RegisterCommands();
    RegisterMenu();
    RegisterTabSpawner();
}

void FPokeSharpEditorModule::ShutdownModule()
{
    FPokeSharpStyle::Shutdown();
    UnregisterTabSpawner();
}

void FPokeSharpEditorModule::RegisterCommands()
{
    FPokeSharpEditorCommands::Register();
    PokeSharpCommands = MakeShared<FUICommandList>();

    const auto &Commands = FPokeSharpEditorCommands::Get();
    PokeSharpCommands->MapAction(Commands.GetImportPbsData(),
                                 FExecuteAction::CreateStatic(&FPokeSharpEditorModule::ImportPbsData));
    PokeSharpCommands->MapAction(Commands.GetExportPbsData(),
                                 FExecuteAction::CreateStatic(&FPokeSharpEditorModule::ExportPbsData));
    PokeSharpCommands->MapAction(Commands.GetEditData(),
                                 FExecuteAction::CreateStatic(&FPokeSharpEditorModule::EditData));

    const auto &LevelEditorModule = FModuleManager::GetModuleChecked<FLevelEditorModule>("LevelEditor");
    const auto CommandList = LevelEditorModule.GetGlobalLevelEditorActions();
    CommandList->Append(PokeSharpCommands.ToSharedRef());
}

void FPokeSharpEditorModule::RegisterMenu() const
{
    auto *ToolbarMenu = UToolMenus::Get()->ExtendMenu("LevelEditor.LevelEditorToolBar.PlayToolBar");
    auto &Section = ToolbarMenu->FindOrAddSection("PluginTools");

    const auto Entry = FToolMenuEntry::InitComboButton(
        "PokeSharp", FUIAction(), FOnGetContent::CreateLambda([this] { return GeneratePokeSharpMenu(); }),
        LOCTEXT("PokeSharp_Label", "PokeSharp"), LOCTEXT("PokeSharp_Tooltip", "List of all PokeSharp actions"),
        TAttribute<FSlateIcon>::CreateLambda(
            [] { return FSlateIcon(FPokeSharpStyle::GetStyleSetName(), "PokeSharp.Toolbar"); }));
    Section.AddEntry(Entry);
}

TSharedRef<SWidget> FPokeSharpEditorModule::GeneratePokeSharpMenu() const
{
    const auto &Commands = FPokeSharpEditorCommands::Get();
    FMenuBuilder MenuBuilder(true, PokeSharpCommands);

    MenuBuilder.BeginSection("Data", LOCTEXT("Data", "Data"));

    MenuBuilder.AddMenuEntry(Commands.GetImportPbsData(), NAME_None, TAttribute<FText>(), TAttribute<FText>(),
                             FSlateIcon(FAppStyle::Get().GetStyleSetName(), "Icons.Toolbar.Import"));
    MenuBuilder.AddMenuEntry(Commands.GetExportPbsData(), NAME_None, TAttribute<FText>(), TAttribute<FText>(),
                             FSlateIcon(FAppStyle::Get().GetStyleSetName(), "Icons.Toolbar.Export"));
    MenuBuilder.AddMenuEntry(Commands.GetEditData(), NAME_None, TAttribute<FText>(), TAttribute<FText>(),
                             FSlateIcon(FAppStyle::Get().GetStyleSetName(), "EditorPreferences.TabIcon"));

    MenuBuilder.EndSection();

    return MenuBuilder.MakeWidget();
}

void FPokeSharpEditorModule::ImportPbsData()
{
    // TODO: Implement me
}

void FPokeSharpEditorModule::ExportPbsData()
{
    // TODO: Implement me
}

void FPokeSharpEditorModule::EditData()
{
    FGlobalTabmanager::Get()->TryInvokeTab(PokeSharpEditorTabName);
}

void FPokeSharpEditorModule::RegisterTabSpawner()
{
    FGlobalTabmanager::Get()
        ->RegisterNomadTabSpawner(PokeSharpEditorTabName,
                                  FOnSpawnTab::CreateStatic(&FPokeSharpEditorModule::SpawnPokeSharpEditorTab))
        .SetDisplayName(LOCTEXT("PokeSharpEditorTabTitle", "PokeSharp Data Editor"))
        .SetTooltipText(LOCTEXT("PokeSharpEditorTabTooltip", "Edit PokeSharp data"))
        .SetIcon(FSlateIcon(FPokeSharpStyle::GetStyleSetName(), "PokeSharp.Toolbar"));
}

void FPokeSharpEditorModule::UnregisterTabSpawner()
{
    FGlobalTabmanager::Get()->UnregisterNomadTabSpawner(PokeSharpEditorTabName);
}

TSharedRef<SDockTab> FPokeSharpEditorModule::SpawnPokeSharpEditorTab(const FSpawnTabArgs &SpawnTabArgs)
{
    return SNew(SDockTab).TabRole(NomadTab)[SNew(SPokeSharpEditor)];
}

#undef LOCTEXT_NAMESPACE

IMPLEMENT_MODULE(FPokeSharpEditorModule, PokeSharpEditor)