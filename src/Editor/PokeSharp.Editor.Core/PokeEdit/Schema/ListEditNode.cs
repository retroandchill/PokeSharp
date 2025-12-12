using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

[JsonPolymorphic]
[JsonDerivedType(typeof(ListSetNode), "Set")]
[JsonDerivedType(typeof(ListAddNode), "Add")]
[JsonDerivedType(typeof(ListInsertNode), "Insert")]
[JsonDerivedType(typeof(ListRemoveNode), "Remove")]
[JsonDerivedType(typeof(ListSwapNode), "Swap")]
public abstract record ListEditNode;

public sealed record ListSetNode(int Index, DiffNode Change) : ListEditNode;

public sealed record ListAddNode(JsonNode NewValue) : ListEditNode;

public sealed record ListInsertNode(int Index, JsonNode NewValue) : ListEditNode;

public sealed record ListRemoveNode(int Index) : ListEditNode;

public sealed record ListSwapNode(int IndexA, int IndexB) : ListEditNode;
