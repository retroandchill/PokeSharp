using System.Collections.Immutable;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public abstract record OptionSourceDefinition;

public sealed record StaticOptionSourceDefinition : OptionSourceDefinition
{
    public required ImmutableArray<OptionItemDefinition> Options { get; init; } = [];
}

public sealed record DynamicOptionSourceDefinition : OptionSourceDefinition
{
    public required Name SourceId { get; init; }
}