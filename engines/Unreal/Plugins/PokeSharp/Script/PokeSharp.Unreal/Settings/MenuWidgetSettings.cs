using PokeSharp.Unreal.UI.Pause;
using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.DeveloperSettings;

namespace PokeSharp.Unreal.Settings;

[UClass]
public partial class UMenuWidgetSettings : UDeveloperSettings
{
    [UProperty(PropertyFlags.EditDefaultsOnly, Category = "Menu Widgets")]
    public partial TSoftClassPtr<UPauseMenuWidget> PauseMenuWidgetClass { get; }
}
