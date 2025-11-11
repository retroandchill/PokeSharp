using System.Collections.Immutable;
using System.Reflection;

namespace PokeSharp.Compiler.Core.Schema;

/// <summary>
/// Defines the data type for a PBS field
/// </summary>
public enum PbsFieldType
{
    /// <summary>Integer (can be negative)</summary>
    Integer,

    /// <summary>Unsigned integer (positive or zero)</summary>
    UnsignedInteger,

    /// <summary>Positive integer (must be greater than 0)</summary>
    PositiveInteger,

    /// <summary>Hexadecimal number</summary>
    Hexadecimal,

    /// <summary>Floating point number</summary>
    Float,

    /// <summary>Boolean value</summary>
    Boolean,

    /// <summary>Name (alphanumeric + underscore, not starting with number)</summary>
    Name,

    /// <summary>String value</summary>
    String,

    /// <summary>Unformatted text (multiline, takes rest of line)</summary>
    UnformattedText,

    /// <summary>Symbol (like Name but converted to symbol/identifier)</summary>
    Symbol,

    /// <summary>Enumerable value (must match enum or GameData type)</summary>
    Enumerable,

    /// <summary>Enumerable or integer (flexible type)</summary>
    EnumerableOrInteger,
}

/// <summary>
/// Defines how a field should be structured
/// </summary>
public enum PbsFieldStructure
{
    /// <summary>Single value</summary>
    Single,

    /// <summary>Array of values (prefix with *)</summary>
    Array,

    /// <summary>Repeating field (prefix with ^)</summary>
    Repeating,
}

public readonly record struct SchemaTypeData(
    PbsFieldType Type,
    bool IsOptional = false,
    Type? EnumType = null,
    bool AllowNone = false
);

public record SchemaEntry(PropertyInfo Property, ImmutableArray<SchemaTypeData> TypeEntries)
{
    public string PropertyName => Property.Name;

    public PbsFieldStructure FieldStructure { get; init; } = PbsFieldStructure.Single;
}
