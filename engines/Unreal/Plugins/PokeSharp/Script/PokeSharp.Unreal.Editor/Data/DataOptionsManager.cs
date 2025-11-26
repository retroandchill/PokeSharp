using PokeSharp.Core.Strings;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Editor.Data;

public static class DataOptionsManager
{
    private static readonly Dictionary<FName, INameOptionsSource> Sources = new()
    {
        [nameof(GrowthRate)] = new NameOptionsSource<GrowthRate>(),
            [nameof(GenderRatio)] = new NameOptionsSource<GenderRatio>(),
            [nameof(EggGroup)] = new NameOptionsSource<EggGroup>(),
            [nameof(BodyShape)] = new NameOptionsSource<BodyShape>(),
            [nameof(BodyColor)] = new NameOptionsSource<BodyColor>(),
            [nameof(Habitat)] = new NameOptionsSource<Habitat>(),
            [nameof(Evolution)] = new NameOptionsSource<Evolution>(),
            [nameof(Stat)] = new NameOptionsSource<Stat>(),
            [nameof(Nature)] = new NameOptionsSource<Nature>(),
            [nameof(Status)] = new NameOptionsSource<Status>(),
            [nameof(Weather)] = new NameOptionsSource<Weather>(),
            [nameof(EncounterType)] = new NameOptionsSource<EncounterType>(),
            [nameof(GameEnvironment)] = new NameOptionsSource<GameEnvironment>(),
            [nameof(BattleWeather)] = new NameOptionsSource<BattleWeather>(),
            [nameof(BattleTerrain)] = new NameOptionsSource<BattleTerrain>(),
            [nameof(Target)] = new NameOptionsSource<Target>(),
            [nameof(PokemonType)] = new NameOptionsSource<PokemonType>(),
            [nameof(Ability)] = new NameOptionsSource<Ability>(),
            [nameof(Move)] = new NameOptionsSource<Move>(),
            [nameof(Item)] = new NameOptionsSource<Item>(),
            [nameof(BerryPlant)] = new NameOptionsSource<BerryPlant>(),
            [nameof(Species)] = new SpeciesOptionSource(),
            [nameof(Ribbon)] = new NameOptionsSource<Ribbon>(),
            [nameof(TrainerType)] = new NameOptionsSource<TrainerType>()
    };

    public static (int Count, IEnumerable<FName> Names) GetNameOptions(FName name) => Sources[name].Options;
}