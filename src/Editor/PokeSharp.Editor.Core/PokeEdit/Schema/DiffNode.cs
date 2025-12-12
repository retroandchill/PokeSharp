using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

[JsonPolymorphic]
[JsonDerivedType(typeof(ValueSetNode), "ValueSet")]
[JsonDerivedType(typeof(ValueResetNode), "ValueReset")]
[JsonDerivedType(typeof(ObjectDiffNode), "Object")]
[JsonDerivedType(typeof(ListDiffNode), "List")]
[JsonDerivedType(typeof(DictionaryDiffNode), "Dictionary")]
public abstract record DiffNode;

public sealed record ValueSetNode(JsonNode NewValue) : DiffNode;

public sealed record ValueResetNode : DiffNode;

public sealed record ObjectDiffNode(ImmutableDictionary<Name, DiffNode> Properties) : DiffNode;

public sealed record ListDiffNode(ImmutableArray<ListEditNode> Edits) : DiffNode;

public sealed record DictionaryDiffNode(ImmutableArray<DictionaryEditNode> Edits) : DiffNode;
