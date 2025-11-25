using UnrealSharp.Attributes;
using UnrealSharp.PokeSharpCore;

namespace PokeSharp.Unreal.Main.UI;

[UClass(ClassFlags.DefaultConfig, ConfigCategory = "Game")]
[UMetaData("DisplayName", "Pokemon UI Manager Subsystem")]
public partial class UPokemonUIManagerSubsystem : UGameUIManagerSubsystem;
