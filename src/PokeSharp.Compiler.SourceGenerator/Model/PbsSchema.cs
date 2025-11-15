using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using PokeSharp.Compiler.Core.Schema;

namespace PokeSharp.Compiler.SourceGenerator.Model;

/// <summary>
/// Defines how a field should be structured
/// </summary>
internal enum PbsFieldStructure
{
    /// <summary>Single value</summary>
    Single,

    /// <summary>Array of values (prefix with *)</summary>
    Array,

    /// <summary>Repeating field (prefix with ^)</summary>
    Repeating,
}

internal readonly record struct PbsSchemaTypeData(
    PbsFieldType Type,
    ITypeSymbol TargetType,
    bool IsOptional = false,
    ITypeSymbol? EnumType = null,
    bool AllowNone = false
);

internal record PbsSchemaEntry(IPropertySymbol Property, ImmutableArray<PbsSchemaTypeData> TypeEntries)
{
    public string PropertyName => Property.Name;

    public PbsFieldStructure FieldStructure { get; init; } = PbsFieldStructure.Single;
}

internal readonly record struct PbsSchemaKey(string KeyName, PbsSchemaEntry Entry);

internal readonly record struct PbsSchema(PbsSchemaEntry SectionName, ImmutableArray<PbsSchemaKey> Keys);
