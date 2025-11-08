using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;

namespace PokeSharp.Compiler.Model;

[PbsData("types")]
public record PokemonTypeInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }
    
    public Text Name { get; init; }
    
    public int IconPosition { get; init; }
    
    public bool IsSpecialType { get; init; }
    
    public bool IsPseudoType { get; init; }
    
    public ImmutableArray<Name> Weaknesses { get; init; } = [];
    
    public ImmutableArray<Name> Resistances { get; init; } = [];
    
    public ImmutableArray<Name> Immunities { get; init; } = [];
    
    public IReadOnlySet<Name> Flags { get; init; } = ImmutableHashSet<Name>.Empty;
}