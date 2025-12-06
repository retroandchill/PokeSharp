using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

[JsonPolymorphic]
[JsonDerivedType(typeof(BoolFieldDefinition), "Bool")]
[JsonDerivedType(typeof(TextFieldDefinition), "Text")]
[JsonDerivedType(typeof(NumberFieldDefinition<sbyte>), "Int8")]
[JsonDerivedType(typeof(NumberFieldDefinition<short>), "Int16")]
[JsonDerivedType(typeof(NumberFieldDefinition<int>), "Int32")]
[JsonDerivedType(typeof(NumberFieldDefinition<long>), "Int64")]
[JsonDerivedType(typeof(NumberFieldDefinition<byte>), "Int8")]
[JsonDerivedType(typeof(NumberFieldDefinition<ushort>), "Int16")]
[JsonDerivedType(typeof(NumberFieldDefinition<uint>), "Int32")]
[JsonDerivedType(typeof(NumberFieldDefinition<ulong>), "Int64")]
[JsonDerivedType(typeof(NumberFieldDefinition<float>), "Float")]
[JsonDerivedType(typeof(NumberFieldDefinition<double>), "Double")]
[JsonDerivedType(typeof(ChoiceFieldDefinition), "Choice")]
[JsonDerivedType(typeof(ObjectFieldDefinition), "Object")]
[JsonDerivedType(typeof(ListFieldDefinition), "List")]
[JsonDerivedType(typeof(DictionaryFieldDefinition), "Dictionary")]
[JsonDerivedType(typeof(OptionalFieldDefinition), "Optional")]
public abstract record FieldDefinition
{
    public required FieldPathSegment FieldId { get; init; }
    public required Text Label { get; init; }
    public Text Tooltip { get; init; }
    public Text Category { get; init; }

    public bool IsDefaultValue { get; init; } = true;
}

public sealed record BoolFieldDefinition : FieldDefinition
{
    public bool CurrentValue { get; init; }
}

public sealed record TextFieldDefinition : FieldDefinition
{
    public int? MaxLength { get; init; }
    public string? Regex { get; init; }
    public bool AllowEmpty { get; init; } = true;
    public bool AllowMultiline { get; init; }
    public bool IsLocalizable { get; init; }

    public string CurrentValue { get; init; } = "";
}

public sealed record NumberFieldDefinition<T> : FieldDefinition
    where T : struct, INumber<T>
{
    public T? MinValue { get; init; }
    public T? MaxValue { get; init; }
    public T? Step { get; init; }

    /// <summary>
    /// Either the number of decimal places to display, or the number of fixed-point decimal places to interpret the
    /// field as having depending of whether T is a floating point value.
    /// </summary>
    public int? DecimalPlaces { get; init; }

    public bool IsFloatingPoint => typeof(T) == typeof(double) || typeof(T) == typeof(float);

    public T CurrentValue { get; init; }
}

public sealed record ChoiceFieldDefinition : FieldDefinition
{
    public bool AllowNone { get; init; }

    public required OptionSourceDefinition Options { get; init; }

    public required JsonNode CurrentValue { get; init; }
}

public sealed record ObjectFieldDefinition : FieldDefinition
{
    public ImmutableArray<FieldDefinition> Fields { get; init; } = [];
}

public abstract record CollectionFieldDefinition : FieldDefinition
{
    public bool FixedSize { get; init; }
    public int? MinSize { get; init; }
    public int? MaxSize { get; init; }
}

public sealed record ListFieldDefinition : CollectionFieldDefinition
{
    public ImmutableArray<FieldDefinition> ItemFields { get; init; } = [];
}

public readonly record struct DictionaryFieldPair(FieldDefinition Key, FieldDefinition Value);

public sealed record DictionaryFieldDefinition : CollectionFieldDefinition
{
    public ImmutableArray<DictionaryFieldPair> Pairs { get; init; } = [];
}

public sealed record OptionalFieldDefinition : FieldDefinition
{
    public FieldDefinition? ValueField { get; init; }
}
