using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(PropertySegment), "Property")]
[JsonDerivedType(typeof(ListIndexSegment), "ListIndex")]
[JsonDerivedType(typeof(DictionaryKeySegment), "DictionaryKey")]
public abstract record FieldPathSegment;

public sealed record PropertySegment(Name Name) : FieldPathSegment;

public sealed record ListIndexSegment(int Index) : FieldPathSegment;

public sealed record DictionaryKeySegment(JsonNode Key) : FieldPathSegment;

public readonly record struct FieldPath(ImmutableArray<FieldPathSegment> Segments)
{
    public override string ToString() =>
        string.Join(
            ".",
            Segments.Select(segment =>
                segment switch
                {
                    PropertySegment p => p.Name.ToString(),
                    ListIndexSegment i => $"[{i.Index}]",
                    DictionaryKeySegment dk => $"[{dk.Key.ToJsonString()}]",
                    _ => throw new NotSupportedException(),
                }
            )
        );
}
