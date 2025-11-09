using System.Collections.Immutable;
using JetBrains.Annotations;

namespace PokeSharp.Compiler.Core.Schema;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
[MeansImplicitUse]
public sealed class PbsDataAttribute(string baseFilename) : Attribute
{
    public string BaseFilename { get; } = baseFilename;

    public bool IsOptional { get; init; } = false;
}

[AttributeUsage(AttributeTargets.Property)]
public abstract class PbsFieldBaseAttribute : Attribute
{
    public abstract string Name { get; }
}

public sealed class PbsSectionNameAttribute : PbsFieldBaseAttribute
{
    public override string Name => "SectionName";
}

public sealed class PbsKeyNameAttribute(string key) : PbsFieldBaseAttribute
{
    public override string Name { get; } = key;
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class PbsTypeAttribute(PbsFieldType fieldType) : Attribute
{
    public PbsFieldType FieldType { get; } = fieldType;
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class PbsKeyRepeatAttribute : Attribute;