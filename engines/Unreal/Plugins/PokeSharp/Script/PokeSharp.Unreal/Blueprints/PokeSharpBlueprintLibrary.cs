using PokeSharp.Core;
using PokeSharp.PokemonModel;
using PokeSharp.Trainers;
using PokeSharp.Unreal.Core.Strings;
using UnrealSharp.Attributes;
using UnrealSharp.Core;
using UnrealSharp.Engine;

namespace PokeSharp.Unreal.Blueprints;

[UClass]
public partial class UPokeSharpBlueprintLibrary : UBlueprintFunctionLibrary
{
    [UFunction(FunctionFlags.BlueprintCallable, Category = "Game")]
    public static void StartNewGame()
    {
        GameGlobal.GameState.StartNew();
    }

    [UFunction(FunctionFlags.BlueprintCallable, Category = "Pokémon")]
    [UMetaData("DisplayName", "Add Pokémon To Party")]
    public static void AddPokemonToParty(
        [UMetaData("GetOptions", "OptionSelectionSourceFunctions.GetSpecies")] FName species,
        int level = 5
    )
    {
        // TODO: We're going to eventually add a new method on the main library that this will call to instead.
        GameGlobal.PlayerTrainer.Party.Add(new Pokemon(species.ToPokeSharpName(), level));
    }
}
