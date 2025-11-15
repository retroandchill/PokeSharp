#if POKESHARP_COMPILER_GENERATOR
using RhoMicro.CodeAnalysis;
#endif

// ReSharper disable once CheckNamespace
namespace PokeSharp.Compiler.Core.Schema;

/// <summary>
/// Defines the data type for a PBS field
/// </summary>
internal enum PbsFieldType
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

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
#if POKESHARP_COMPILER_GENERATOR
[IncludeFile]
#endif
internal sealed class PbsDataAttribute(string baseFilename) : Attribute
{
    public string BaseFilename { get; } = baseFilename;

    public bool IsOptional { get; init; } = false;
}

[AttributeUsage(AttributeTargets.Property)]
internal sealed class PbsSectionNameAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
internal sealed class PbsKeyNameAttribute(string key) : Attribute
{
    public string Name { get; } = key;
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
internal sealed class PbsTypeAttribute(PbsFieldType fieldType) : Attribute
{
    public PbsFieldType FieldType { get; } = fieldType;

    public Type? EnumType { get; init; }

    public bool AllowNone { get; init; }

    public int FixedSize { get; init; } = -1;

    public bool FixedSizeIsMax { get; init; }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
internal sealed class PbsKeyRepeatAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
internal sealed class PbsIgnoreAttribute : Attribute;
