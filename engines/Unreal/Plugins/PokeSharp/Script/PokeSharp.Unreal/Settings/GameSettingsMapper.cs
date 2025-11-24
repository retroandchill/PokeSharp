using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.Settings;
using PokeSharp.Trainers;
using PokeSharp.Unreal.Core;
using PokeSharp.Unreal.Core.Configuration;
using PokeSharp.Unreal.Core.Interop;
using PokeSharp.Unreal.Core.Strings;
using Riok.Mapperly.Abstractions;
using UnrealSharp;
using UnrealSharp.Core;
using UnrealSharp.PokeSharp;

namespace PokeSharp.Unreal.Settings;

[RegisterSingleton]
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class GameSettingsMapper : IUnrealOptionsMapper<UPokeSharpSettings, GameSettings>
{
    // These warnings are pointless when the source file is generated and lacks nullability annotations. But we know
    // for a fact that the source object is never null.
    [MapperIgnoreTarget(nameof(GameSettings.Version))]
    public partial GameSettings Map(UPokeSharpSettings settings);

    [MapProperty(
        nameof(FRoamingSpecies.BattleMusic),
        nameof(RoamingSpecies.BattleMusic),
        Use = nameof(MapOptionalString)
    )]
    private partial RoamingSpecies Map(FRoamingSpecies roamingSpecies);

    [MapProperty(nameof(FPokedexName.Region), nameof(PokedexName.Region), Use = nameof(MapRegion))]
    private partial PokedexName Map(FPokedexName pokedexName);

    private static int? MapOptionalInt(TOptional<int> optional) => optional.HasValue ? optional.Value : null;

    private static int MapRegion(TOptional<int> optional) => optional.OrElse(Pokedex.NationalDex);

    private static string? MapOptionalString(TOptional<string> optional) => optional.HasValue ? optional.Value : null;

    private static Name MapName(FName name) => name.ToPokeSharpName();

    private static Text MapText(FText text) => text.ToPokeSharpText();

    private static ImmutableArray<int> MapRoamingAreasList(FRoamingAreasList roamingAreasList)
    {
        return [.. roamingAreasList.Areas];
    }
}
