using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

[GameDataEntity(DataPath = "ribbons")]
public partial record Ribbon
{
    public required Name Id { get; init; }
    
    public Text Name { get; init; } = TextConstants.Unnamed;
    
    public Text Description { get; init; } = TextConstants.ThreeQuestions;
    
    public IReadOnlySet<Name> Flags { get; init; } = ImmutableHashSet<Name>.Empty;
    
    public bool HasFlag(Name flag) => Flags.Contains(flag);
}