using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;

namespace PokeSharp.Compiler.Model;

[PbsData("types")]
public record PokemonTypeInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; init; } = TextConstants.Unnamed;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int IconPosition { get; init; }

    public bool IsSpecialType { get; init; }

    public bool IsPseudoType { get; init; }

    public ImmutableArray<Name> Weaknesses { get; init; } = [];

    public ImmutableArray<Name> Resistances { get; init; } = [];

    public ImmutableArray<Name> Immunities { get; init; } = [];

    public IReadOnlySet<string> Flags { get; init; } = ImmutableHashSet<string>.Empty;
}
