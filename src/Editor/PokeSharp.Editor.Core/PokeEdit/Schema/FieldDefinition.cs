using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public abstract record FieldDefinition
{
    public required Name FieldId { get; init; }
    public required Text Label { get; init; }
    public Text Tooltip { get; init; }
    public Name Category { get; init; }
}

public sealed record BoolFieldDefinition : FieldDefinition;

public sealed record TextFieldDefinition : FieldDefinition
{
    public int? MaxLength { get; init; }
    public string? Regex { get; init; }
    public bool AllowEmpty { get; init; } = true;
    public bool IsLocalizable { get; init; } = false;
}

public sealed record IntFieldDefinition : FieldDefinition
{
    public int? MinValue { get; init; }
    public int? MaxValue { get; init; }
    public int? Step { get; init; }
}

public sealed record FloatFieldDefinition : FieldDefinition
{
    public double? MinValue { get; init; }
    public double? MaxValue { get; init; }
    public double? Step { get; init; }
}

public sealed record ChoiceFieldDefinition : FieldDefinition
{
    public bool AllowNone { get; init; }
    
    public required OptionSourceDefinition Options { get; init; }
}

public sealed record ObjectFieldDefinition : FieldDefinition
{
    public required Name ObjectTypeId { get; init; }
}

public abstract record CollectionFieldDefinition : FieldDefinition
{
    public bool FixedSize { get; init; }
    public int? MinSize { get; init; }
    public int? MaxSize { get; init; }
}

public sealed record ListFieldDefinition : CollectionFieldDefinition
{
    public required FieldDefinition ItemField { get; init; }
}

public sealed record DictionaryFieldDefinition : CollectionFieldDefinition
{
    public required FieldDefinition KeyField { get; init; }
    public required FieldDefinition ValueField { get; init; }
}

public sealed record OptionalFieldDefinition : FieldDefinition
{
    public required FieldDefinition ValueField { get; init; }
}