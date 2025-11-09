using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

[GameDataEntity(DataPath = "abilities")]
public partial record Ability
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    public required Text Description { get; init; }

    public required IReadOnlySet<Name> Flags { get; init; }

    public bool HasFlag(Name flag) => Flags.Contains(flag);
}
