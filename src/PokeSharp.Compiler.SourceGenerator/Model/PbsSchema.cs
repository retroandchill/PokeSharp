using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.SourceGenerator.Utilities;

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
    int Index,
    string? DefaultValue = null,
    ITypeSymbol? EnumType = null,
    bool AllowNone = false
)
{
    public string? ParseMethod
    {
        get
        {
            var targetTypeStr = GetTargetTypeString(TargetType);
            var enumTypeStr = EnumType?.ToDisplayString(NullableFlowState.NotNull) ?? targetTypeStr;
            return Type switch
            {
                PbsFieldType.Integer => $"CsvParser.ParseInt<{targetTypeStr}>",
                PbsFieldType.UnsignedInteger => $"CsvParser.ParseUnsigned<{targetTypeStr}>",
                PbsFieldType.PositiveInteger => $"CsvParser.ParsePositive<{targetTypeStr}>",
                PbsFieldType.Hexadecimal => $"CsvParser.ParseHex<{targetTypeStr}>",
                PbsFieldType.Float => $"CsvParser.ParseFloat<{targetTypeStr}>",
                PbsFieldType.Boolean => "CsvParser.ParseBoolean",
                PbsFieldType.Name => "CsvParser.ParseName",
                PbsFieldType.String or PbsFieldType.UnformattedText => null,
                PbsFieldType.Symbol => "CsvParser.ParseSymbol",
                PbsFieldType.Enumerable => TargetType.TypeKind == TypeKind.Enum
                    ? $"CsvParser.ParseEnumField<{targetTypeStr}>"
                    : $"CsvParser.ParseDataEnum<{enumTypeStr}, {targetTypeStr}>",
                PbsFieldType.EnumerableOrInteger => TargetType.TypeKind == TypeKind.Enum
                    ? $"CsvParser.ParseEnumOrInt<{targetTypeStr}>"
                    : null,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }

    public static string GetTargetTypeString(ITypeSymbol type)
    {
        if (
            type is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol
            && namedTypeSymbol.ConstructUnboundGenericType().ToDisplayString() == "T?"
        )
        {
            return GetTargetTypeString(namedTypeSymbol.TypeArguments[0]);
        }

        return type.ToDisplayString(NullableFlowState.NotNull);
    }

    public bool NeedsParsing => ParseMethod is not null;

    public bool IsOptional => DefaultValue is not null;

    public bool NeedsConversion => TargetType.ToDisplayString(NullableFlowState.NotNull) == GeneratorConstants.Text;

    public bool IsLast { get; init; } = true;
}

internal record PbsSchemaEntry(string KeyName, IPropertySymbol Property, ImmutableArray<PbsSchemaTypeData> TypeEntries)
{
    public string Type => PbsSchemaTypeData.GetTargetTypeString(Property.Type);

    public string Name => Property.Name;

    public string ConcreteType
    {
        get
        {
            var elementType = ElementType;
            return elementType is not null ? $"List<{elementType}>" : Type;
        }
    }

    public string? ElementType
    {
        get
        {
            if (
                Property.Type is INamedTypeSymbol
                {
                    IsGenericType: true,
                    TypeParameters.Length: 1,
                    Name: "IReadOnlyCollection"
                        or "ICollection"
                        or "IEnumerable"
                        or "IReadOnlyList"
                        or "IList"
                        or "List"
                } genericType
            )
            {
                return genericType.TypeArguments[0].ToDisplayString(NullableFlowState.NotNull);
            }

            return null;
        }
    }

    public string ParsedType => AppearsOnce ? Type : ElementType ?? Type;

    public bool IsSectionName { get; init; }

    public bool IsFixedSize { get; init; }

    public bool IsRequired => IsSectionName || Property.IsRequired;

    public bool IsInitOnly => Property.SetMethod is { IsInitOnly: true };

    public PbsFieldStructure FieldStructure { get; init; } = PbsFieldStructure.Single;

    public bool AppearsOnce => FieldStructure != PbsFieldStructure.Repeating;

    public bool IsSimpleType => TypeEntries.Length == 1 && FieldStructure == PbsFieldStructure.Single;

    public bool IsCollection => FieldStructure == PbsFieldStructure.Array || IsFixedSize;

    public bool NeedsSubsections => TypeEntries.Length > 1;

    private bool? _needsSectionName;
    public bool NeedsSectionName
    {
        get
        {
            if (_needsSectionName.HasValue)
                return _needsSectionName.Value;

            _needsSectionName = TypeEntries.Any(t => t.NeedsConversion);
            return _needsSectionName.Value;
        }
    }

    public int RequiredLength => TypeEntries.Count(e => !e.IsOptional);
    public int MaxLength => TypeEntries.Length;

    public bool NoOptionalParameters => TypeEntries.All(e => !e.IsOptional);
}

internal record PbsSchema(
    ITypeSymbol ModelType,
    ImmutableArray<PbsSchemaEntry> Properties,
    string FilePath,
    bool IsOptional
)
{
    public string Namespace => ModelType.ContainingNamespace.ToDisplayString();

    public string ClassName => ModelType.Name;

    public string IsOptionalString => IsOptional ? "true" : "false";

    public string DeclaredAccessibility =>
        ModelType.DeclaredAccessibility switch
        {
            Accessibility.NotApplicable => "",
            Accessibility.Private => "private ",
            Accessibility.ProtectedAndInternal => "private protected ",
            Accessibility.Protected => "protected ",
            Accessibility.Internal => "internal ",
            Accessibility.ProtectedOrInternal => "protected internal ",
            Accessibility.Public => "public ",
            _ => throw new ArgumentOutOfRangeException(),
        };

    public string ObjectType
    {
        get
        {
            if (ModelType.IsRecord)
            {
                return ModelType.IsValueType ? "record struct" : "record";
            }

            return ModelType.IsValueType ? "struct" : "class";
        }
    }
}
