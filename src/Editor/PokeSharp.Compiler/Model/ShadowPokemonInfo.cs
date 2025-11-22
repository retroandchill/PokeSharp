using System.Collections.Immutable;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public readonly record struct ShadowPokemonKey(
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Species))] Name Species,
    [PbsType(PbsFieldType.PositiveInteger)] int Form = 0
)
{
    public override string ToString() => Form > 0 ? $"{Species},{Form}" : Species.ToString();
}

[PbsData("shadow_pokemon", IsOptional = true)]
public partial class ShadowPokemonInfo
{
    [PbsSectionName]
    public required ShadowPokemonKey Id { get; init; }

    public int GaugeSize { get; set; } = ShadowPokemon.MaxGaugeSize;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(PokemonType))]
    public List<Name> Moves { get; set; } = [];

    public List<string> Flags { get; set; } = [];
}
