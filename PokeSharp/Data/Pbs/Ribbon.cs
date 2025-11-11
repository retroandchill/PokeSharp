using System.Collections.Immutable;
using MessagePack;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

[GameDataEntity(DataPath = "ribbons")]
[MessagePackObject(true)]
public partial record Ribbon
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    public required Text Description { get; init; }

    public required int IconPosition { get; init; }

    public required ImmutableArray<Name> Flags { get; init; }

    public bool HasFlag(Name flag) => Flags.Contains(flag);
}
