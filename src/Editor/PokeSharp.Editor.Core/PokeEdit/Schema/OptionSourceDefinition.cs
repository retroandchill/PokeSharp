using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public readonly record struct OptionItemDefinition(JsonNode Value, Text Label);

[JsonPolymorphic]
[JsonDerivedType(typeof(StaticOptionSourceDefinition), "Static")]
[JsonDerivedType(typeof(DynamicOptionSourceDefinition), "Dynamic")]
public abstract record OptionSourceDefinition;

public sealed record StaticOptionSourceDefinition : OptionSourceDefinition
{
    public required ImmutableArray<OptionItemDefinition> Options { get; init; } = [];
}

public sealed record DynamicOptionSourceDefinition : OptionSourceDefinition
{
    public required Name SourceId { get; init; }
}
