using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target, PreferParameterlessConstructors = false)]
public static partial class TrainerMapper
{
    public static partial EnemyTrainer ToGameData(this EnemyTrainerInfo dto);

    [MapProperty(nameof(TrainerPokemonInfo.Name), nameof(TrainerPokemon.Nickname))]
    [MapProperty(nameof(TrainerPokemonInfo.IV), nameof(TrainerPokemon.IVs))]
    [MapProperty(nameof(TrainerPokemonInfo.EV), nameof(TrainerPokemon.EVs))]
    private static partial TrainerPokemon ToGameData(this TrainerPokemonInfo dto);

    public static partial EnemyTrainerInfo ToDto(this EnemyTrainer entity);

    [MapPropertyFromSource(nameof(TrainerPokemonInfo.Id))]
    [MapProperty(nameof(TrainerPokemon.Nickname), nameof(TrainerPokemonInfo.Name))]
    [MapProperty(nameof(TrainerPokemon.IVs), nameof(TrainerPokemonInfo.IV))]
    [MapProperty(nameof(TrainerPokemon.EVs), nameof(TrainerPokemonInfo.EV))]
    public static partial TrainerPokemonInfo ToDto(this TrainerPokemon entity);

    private static TrainerPokemonKey MapTrainerPokemonKey(TrainerPokemon pokemon) =>
        new(pokemon.Species, pokemon.Level);

    private static IReadOnlyDictionary<Name, int> MapStats(List<int> stats)
    {
        return Stat
            .AllMain.Where(s => s.PbsOrder >= 0)
            .Select(s => (s.Id, Value: stats.Count > s.PbsOrder ? stats[s.PbsOrder] : stats[1]))
            .ToImmutableDictionary(s => s.Id, s => s.Value);
    }

    private static List<int> MapStats(IReadOnlyDictionary<Name, int> stats)
    {
        return stats.OrderBy(s => Stat.Get(s.Key).PbsOrder).Select(x => x.Value).ToList();
    }
}
