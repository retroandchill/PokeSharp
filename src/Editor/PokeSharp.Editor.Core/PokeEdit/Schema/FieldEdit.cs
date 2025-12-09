using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

[JsonPolymorphic]
[JsonDerivedType(typeof(SetValueEdit), "SetValue")]
[JsonDerivedType(typeof(ListAddEdit), "ListAdd")]
[JsonDerivedType(typeof(ListInsertEdit), "ListInsert")]
[JsonDerivedType(typeof(ListRemoveAtEdit), "ListRemoveAt")]
[JsonDerivedType(typeof(ListSwapEdit), "ListSwap")]
[JsonDerivedType(typeof(DictionarySetEntryEdit), "DictionarySetEntry")]
[JsonDerivedType(typeof(DictionaryRemoveEntryEdit), "DictionaryRemoveEntry")]
[JsonDerivedType(typeof(OptionalResetEdit), "OptionalReset")]
public abstract record FieldEdit
{
    public required FieldPath Path { get; init; }
}

public sealed record SetValueEdit : FieldEdit
{
    public required JsonNode NewValue { get; init; }
}

public sealed record ListAddEdit : FieldEdit
{
    public required JsonNode NewItem { get; init; }
}

public sealed record ListInsertEdit : FieldEdit
{
    public required int Index { get; init; }
    public required JsonNode NewItem { get; init; }
}

public sealed record ListRemoveAtEdit : FieldEdit
{
    public required int Index { get; init; }
    public JsonNode? OriginalItem { get; init; }
}

public sealed record ListSwapEdit : FieldEdit
{
    public required int IndexA { get; init; }
    public required int IndexB { get; init; }
}

public sealed record DictionarySetEntryEdit : FieldEdit
{
    public required JsonNode Key { get; init; }
    public required JsonNode NewValue { get; init; }
}

public sealed record DictionaryRemoveEntryEdit : FieldEdit
{
    public required JsonNode Key { get; init; }
    public JsonNode? OriginalValue { get; init; }
}

public sealed record OptionalResetEdit : FieldEdit
{
    public JsonNode? OriginalValue { get; init; }
}
