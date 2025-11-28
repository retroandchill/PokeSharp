using System.Collections.Immutable;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public sealed record TypeDefinition
{
    public required Name Id { get; init; }
    public required Text Name { get; init; }
    public ImmutableArray<FieldDefinition> Fields { get; init; } = [];
}