using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

[JsonPolymorphic]
[JsonDerivedType(typeof(DictionarySetNode), "Set")]
[JsonDerivedType(typeof(DictionaryAddNode), "Add")]
[JsonDerivedType(typeof(DictionaryRemoveNode), "Remove")]
[JsonDerivedType(typeof(DictionaryChangeKeyNode), "ChangeKey")]
public abstract record DictionaryEditNode;

public sealed record DictionarySetNode(JsonNode Key, DiffNode Change) : DictionaryEditNode;

public sealed record DictionaryAddNode(JsonNode Key, JsonNode Value) : DictionaryEditNode;

public sealed record DictionaryRemoveNode(JsonNode Key) : DictionaryEditNode;

public sealed record DictionaryChangeKeyNode(JsonNode OldKey, JsonNode NewKey) : DictionaryEditNode;
