using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using PokeSharp.Compiler.SourceGenerator.Utilities;

namespace PokeSharp.Compiler.SourceGenerator.Model;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal record PbsSchemaEntry
{
    public string KeyName { get; }
    public IPropertySymbol Property { get; }
    public ImmutableArray<PbsSchemaTypeData> TypeEntries { get; }

    public string Type { get; }

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

    public PbsFieldStructure FieldStructure { get; init; } = PbsFieldStructure.Single;

    public bool IsSectionName { get; init; }

    public bool IsFixedSize { get; init; }

    public bool IsRequired => IsSectionName || Property.IsRequired;

    public bool IsInitOnly => Property.SetMethod is { IsInitOnly: true };

    public bool IsNullable => Property.Type.NullableAnnotation == NullableAnnotation.Annotated;

    public bool IsValueType => Property.Type.IsValueType;

    public bool AppearsOnce => FieldStructure != PbsFieldStructure.Repeating;

    public bool IsSimpleType => TypeEntries.Length == 1 && FieldStructure == PbsFieldStructure.Single;

    public bool IsBoolean { get; }

    public bool IsCollection => FieldStructure == PbsFieldStructure.Array || IsFixedSize;

    public bool IsSequence { get; }

    public bool NeedsSubsections => TypeEntries.Length > 1;

    public bool NeedsSectionName { get; }

    public int RequiredLength => TypeEntries.Count(e => !e.IsOptional);
    public int MaxLength => TypeEntries.Length;

    public bool NoOptionalParameters => TypeEntries.All(e => !e.IsOptional);

    public bool HasValidation => HasBuiltInValidation || HasCustomValidation;

    public string? ValidateMethodName => HasBuiltInValidation ? $"Validate_{Name}" : CustomValidateMethod;

    public bool HasBuiltInValidation => IsCollection || IsBoolean;

    public bool HasCustomValidation => CustomValidateMethod is not null;

    public string? CustomValidateMethod
    {
        get;
        init
        {
            field = value;

            ValidateMethodIsStatic =
                field is null || Property.ContainingType.GetMembers(field).OfType<IMethodSymbol>().All(m => m.IsStatic);
        }
    }

    public bool ValidateMethodIsStatic { get; private init; }

    public string? CustomWriteMethod
    {
        get;
        init
        {
            field = value;
            CustomWriteMethodIsStatic =
                field is null || Property.ContainingType.GetMembers(field).OfType<IMethodSymbol>().All(m => m.IsStatic);
        }
    }

    public bool HasCustomWriteMethod => CustomWriteMethod is not null;

    public bool CustomWriteMethodIsStatic { get; private init; }

    public PbsSchemaEntry(string keyName, IPropertySymbol property, ImmutableArray<PbsSchemaTypeData> typeEntries)
    {
        KeyName = keyName;
        Property = property;
        TypeEntries = typeEntries;

        Type = TypeUtils.GetTargetTypeString(property.Type);

        NeedsSectionName = typeEntries.Any(t => t.NeedsConversion);

        IsSequence = property.Type.AllInterfaces.Any(i =>
            i.IsGenericType
            && i.ConstructUnboundGenericType().ToDisplayString() == "System.Collections.Generic.IEnumerable<>"
        );
        IsBoolean = TypeUtils.GetUnderlyingType(property.Type).SpecialType == SpecialType.System_Boolean;
    }
}
