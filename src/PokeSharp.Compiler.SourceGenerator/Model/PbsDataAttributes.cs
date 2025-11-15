using Microsoft.CodeAnalysis;
using PokeSharp.Compiler.Core.Schema;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.Compiler.SourceGenerator.Model;

[AttributeInfoType<PbsDataAttribute>]
internal readonly record struct PbsDataInfo(string BaseFilename)
{
    public bool IsOptional { get; init; } = false;
}

[AttributeInfoType<PbsKeyNameAttribute>]
internal readonly record struct PbsKeyNameInfo(string Name);

[AttributeInfoType<PbsTypeAttribute>]
internal sealed record PbsTypeInfo(PbsFieldType FieldType)
{
    public ITypeSymbol? EnumType { get; init; }

    public bool AllowNone { get; init; }

    public int FixedSize { get; init; } = -1;

    public bool FixedSizeIsMax { get; init; }
}
