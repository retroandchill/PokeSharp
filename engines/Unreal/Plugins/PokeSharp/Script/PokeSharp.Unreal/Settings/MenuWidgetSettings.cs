using PokeSharp.Unreal.UI.Pause;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.DeveloperSettings;
using UnrealSharp.UnrealSharpCore;

namespace PokeSharp.Unreal.Settings;

[UClass(ClassFlags.DefaultConfig, ConfigCategory = "Game")]
public partial class UMenuWidgetSettings : UCSDeveloperSettings
{
    [UProperty(
        PropertyFlags.EditDefaultsOnly | PropertyFlags.BlueprintReadOnly | PropertyFlags.Config,
        Category = "Widgets Classes"
    )]
    public partial TSoftClassPtr<UPauseMenuWidget> PauseMenuWidgetClass { get; }
}
