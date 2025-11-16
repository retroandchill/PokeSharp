using JetBrains.Annotations;
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

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal record PbsSchemaTypeData
{
    public PbsFieldType Type { get; }
    public ITypeSymbol TargetType { get; }
    public ITypeSymbol? EnumType { get; }
    public int Index { get; init; }
    public string? DefaultValue { get; init; }
    public bool AllowNone { get; init; }

    public string? ParseMethod { get; }

    public string? WriteMethod { get; }

    public bool HasWriteMethod => WriteMethod is not null;

    public bool IsString => TargetType.SpecialType == SpecialType.System_String;

    public bool NeedsParsing => ParseMethod is not null;

    public bool IsOptional => DefaultValue is not null;

    public bool NeedsConversion { get; }

    public bool IsLast { get; init; } = true;

    public bool IsUnformatted => Type == PbsFieldType.UnformattedText;

    public PbsSchemaTypeData(PbsFieldType type, ITypeSymbol targetType, ITypeSymbol? enumType = null)
    {
        Type = type;
        TargetType = targetType;
        EnumType = enumType;

        var underlyingType = TypeUtils.GetUnderlyingType(targetType);
        var targetTypeString = TypeUtils.GetTargetTypeString(targetType);

        var enumTypeStr = EnumType?.ToDisplayString(NullableFlowState.NotNull) ?? targetTypeString;
        ParseMethod = Type switch
        {
            PbsFieldType.Integer => $"CsvParser.ParseInt<{targetTypeString}>",
            PbsFieldType.UnsignedInteger => $"CsvParser.ParseUnsigned<{targetTypeString}>",
            PbsFieldType.PositiveInteger => $"CsvParser.ParsePositive<{targetTypeString}>",
            PbsFieldType.Hexadecimal => $"CsvParser.ParseHex<{targetTypeString}>",
            PbsFieldType.Float => $"CsvParser.ParseFloat<{targetTypeString}>",
            PbsFieldType.Boolean => "CsvParser.ParseBoolean",
            PbsFieldType.Name => "CsvParser.ParseName",
            PbsFieldType.String or PbsFieldType.UnformattedText => null,
            PbsFieldType.Symbol => "CsvParser.ParseSymbol",
            PbsFieldType.Enumerable => TargetType.TypeKind == TypeKind.Enum
                ? $"CsvParser.ParseEnumField<{targetTypeString}>"
                : $"CsvParser.ParseDataEnum<{enumTypeStr}, {targetTypeString}>",
            PbsFieldType.EnumerableOrInteger => TargetType.TypeKind == TypeKind.Enum
                ? $"CsvParser.ParseEnumOrInt<{targetTypeString}>"
                : null,
            _ => throw new ArgumentOutOfRangeException(),
        };

        if (Type == PbsFieldType.EnumerableOrInteger && underlyingType.TypeKind == TypeKind.Enum)
        {
            WriteMethod = "CsvWriter.WriteEnumOrIntegerRecord";
        }
        else if (
            underlyingType.SpecialType == SpecialType.System_String
            || TargetType.ToDisplayString(NullableFlowState.NotNull) == GeneratorConstants.Text
        )
        {
            WriteMethod = Type == PbsFieldType.UnformattedText ? null : "TextFormatter.CsvQuote";
        }
        else
        {
            WriteMethod = underlyingType.SpecialType == SpecialType.System_Boolean ? "CsvWriter.WriteBoolean" : null;
        }

        NeedsConversion = TargetType.ToDisplayString(NullableFlowState.NotNull) == GeneratorConstants.Text;
    }
}
