using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public enum ScalarType
{
    Bool,
    Int,
    Float,
    String,
    Choice,
}

public sealed record ScalarSchemaMetadata
{
    // string-ish
    public int? MaxLength { get; init; }
    public string? Regex { get; init; }
    public bool AllowEmpty { get; init; } = true;
    public bool AllowMultiline { get; init; }

    // numeric-ish
    public double? Min { get; init; }
    public double? Max { get; init; }
    public double? Step { get; init; }
    public int? DecimalPlaces { get; init; }

    // enum/choice-ish
    public bool AllowNone { get; init; }
    public OptionSourceDefinition? Options { get; init; }

    // tags for frontend/etc can be added here or in an Extra dictionary
}
