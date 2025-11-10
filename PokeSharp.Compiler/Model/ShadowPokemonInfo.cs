using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public readonly record struct ShadowPokemonKey(
    [property: PbsType(PbsFieldType.Enumerable, EnumType = typeof(Species))] Name Species,
    [property: PbsType(PbsFieldType.PositiveInteger)] int Form = 0
)
{
    public override string ToString() => Form > 0 ? $"{Species},{Form}" : Species.ToString();
}

[PbsData("shadow_pokemon", IsOptional = true)]
public record ShadowPokemonInfo
{
    [PbsSectionName]
    public required ShadowPokemonKey Id { get; init; }

    public int GaugeSize { get; init; } = 4000;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(PokemonType))]
    public ImmutableArray<Name> Moves { get; init; } = [];

    public ImmutableArray<string> Flags { get; init; } = [];
}
