using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class SpeciesMapper
{
    [MapProperty(nameof(SpeciesInfo.Pokedex), nameof(Species.PokedexEntry))]
    [MapProperty(nameof(SpeciesInfo.Moves), nameof(Species.LevelUpMoves))]
    [MapperIgnoreTarget(nameof(Species.PokedexForm))]
    [MapperIgnoreTarget(nameof(Species.MegaStone))]
    [MapperIgnoreTarget(nameof(Species.MegaMove))]
    [MapperIgnoreTarget(nameof(Species.UnmegaForm))]
    [MapperIgnoreTarget(nameof(Species.MegaMessage))]
    public static partial Species ToGameData(this SpeciesInfo dto);

    [MapProperty(nameof(Species.PokedexEntry), nameof(SpeciesInfo.Pokedex))]
    [MapProperty(nameof(Species.LevelUpMoves), nameof(SpeciesInfo.Moves))]
    public static partial SpeciesInfo ToDto(this Species entity);

    private static Name MapToName(SpeciesForm form) => form.Species;

    private static IReadOnlyDictionary<Name, int> MapBaseStats(ImmutableArray<int> stats)
    {
        return Stat
            .AllMain.Where(s => s.PbsOrder >= 0)
            .Select(s => (s.Id, Value: stats.Length > s.PbsOrder ? stats[s.PbsOrder] : 1))
            .ToImmutableDictionary(s => s.Id, s => s.Value);
    }

    private static IReadOnlyDictionary<Name, int> MapEVs(ImmutableArray<EVYieldInfo> stats)
    {
        var statsDictionary = stats.ToDictionary(s => s.Stat, s => s.Amount);

        foreach (var stat in Stat.AllMain)
        {
            statsDictionary.TryAdd(stat.Id, 0);
        }

        return statsDictionary;
    }

    private static ImmutableArray<int> MapStats(IReadOnlyDictionary<Name, int> stats)
    {
        return [.. stats.OrderBy(s => Stat.Get(s.Key).PbsOrder).Select(x => x.Value)];
    }

    private static ImmutableArray<EVYieldInfo> MapEVs(IReadOnlyDictionary<Name, int> stats)
    {
        return [.. stats.Select(x => new EVYieldInfo(x.Key, x.Value)).Where(x => x.Amount > 0)];
    }

    private static EvolutionMethodInfo MapEvolutionMethodInfo(EvolutionInfo evolutionInfo)
    {
        return new EvolutionMethodInfo(
            evolutionInfo.Species,
            evolutionInfo.EvolutionMethod,
            evolutionInfo.Parameter?.ToString()
        );
    }

    private static EvolutionInfo MapEvolutionInfo(EvolutionMethodInfo evolutionInfo)
    {
        return new EvolutionInfo(
            evolutionInfo.Species,
            evolutionInfo.Method,
            evolutionInfo.Parameter
        );
    }

    private static int ConvertDecimalUnit(decimal value)
    {
        return (int)Math.Round(value * 10);
    }
}
