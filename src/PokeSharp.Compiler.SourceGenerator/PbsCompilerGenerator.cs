using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.SourceGenerator.Exceptions;
using PokeSharp.Compiler.SourceGenerator.Model;
using PokeSharp.Compiler.SourceGenerator.Utilities;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.Compiler.SourceGenerator;

[Generator]
public class PbsCompilerGenerator : IIncrementalGenerator
{
    public static readonly DiagnosticDescriptor InvalidPbsType = new(
        "PBS0001",
        "Invalid PBS model type",
        "{0} must be either a class or a struct, and cannot be marked abstract",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingSectionName = new(
        "PBS0002",
        "Missing section name property",
        "{0} does not have a property marked with [PbsSectionName], exactly one property must be marked with this attribute",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor CannotUseKeyNameOnSectionName = new(
        "PBS0003",
        "Cannot use [PbsKeyName] on section name property",
        "{0} cannot be markeed with [PbsKeyName] because it is also marked with [PbsSectionName]",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidType = new(
        "PBS0004",
        "Property type cannot be deserialized",
        "Type {0} cannot be deserialized",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor PropertyTypeMismatch = new(
        "PBS0005",
        "Property type specified not valid",
        "Type {0} has been specified as {1}, which is not valid for type {2}",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor CannotDeduceEnumerableType = new(
        "PBS0006",
        "Cannot deduce enumerable type for property",
        "Property {0} has been specified as enumerable, but no enum type was specified, and it is not of type enum",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor CannotConstructAndDeconstructType = new(
        "PBS0007",
        "Type requires exactly one constructor and one Deconstruct method with the same parameters",
        "Type {0} requires exactly one constructor and one Deconstruct method with the same parameters",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MultipleSectionNames = new(
        "PBS0008",
        "Type requires exactly one section name property",
        "Specified property {0} as [PbsSectionName], but there are multiple properties marked with this attribute",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor UnknownSourceGenerationError = new(
        "PBS0009",
        "An unknown error occurred during source generation",
        "An unknown error occurred during source generation: {0}",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var dataTypes = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                typeof(PbsDataAttribute).FullName!,
                (n, _) => n is TypeDeclarationSyntax,
                (ctx, _) =>
                {
                    var type = (TypeDeclarationSyntax)ctx.TargetNode;
                    return ctx.SemanticModel.GetDeclaredSymbol(type) as INamedTypeSymbol;
                }
            )
            .Where(t => t is not null);

        context.RegisterSourceOutput(dataTypes, Execute!);
    }

    private static void Execute(SourceProductionContext context, INamedTypeSymbol targetType)
    {
        try
        {
            var schema = BuildSchema(targetType);
        }
        catch (PbsSchemaException ex)
        {
            foreach (var diagnostic in ex.Diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
        catch (Exception ex)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(UnknownSourceGenerationError, targetType.Locations[0], ex.ToString())
            );
        }
    }

    private static PbsSchema BuildSchema(ITypeSymbol targetType)
    {
        if (targetType is not { TypeKind: TypeKind.Class or TypeKind.Struct })
        {
            throw new PbsSchemaException(
                $"Type '{targetType.Name}' must be a class or struct.",
                [Diagnostic.Create(InvalidPbsType, targetType.Locations[0], targetType.Name)]
            );
        }

        // Ensure it's not abstract
        if (targetType.IsAbstract)
        {
            throw new PbsSchemaException(
                $"Type '{targetType.Name}' cannot be abstract.",
                [Diagnostic.Create(InvalidPbsType, targetType.Locations[0], targetType.Name)]
            );
        }

        var errors = ImmutableArray.CreateBuilder<Diagnostic>();
        PbsSchemaEntry? sectionName = null;
        var keyValuePairs = ImmutableArray.CreateBuilder<PbsSchemaKey>();

        foreach (
            var property in targetType
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p =>
                    p
                        is {
                            IsStatic: false,
                            DeclaredAccessibility: Accessibility.Public,
                            GetMethod: not null,
                            SetMethod: not null
                        }
                    && !p.HasAttribute<PbsIgnoreAttribute>()
                )
        )
        {
            try
            {
                var (isSectionName, keyName, entry) = GetSchemaEntryInfo(property);
                if (isSectionName)
                {
                    if (sectionName is null)
                    {
                        sectionName = entry;
                    }
                    else
                    {
                        Diagnostic.Create(MultipleSectionNames, property.Locations[0], property.Name);
                    }
                }
                else
                {
                    keyValuePairs.Add(new PbsSchemaKey(keyName, entry));
                }
            }
            catch (PbsSchemaException ex)
            {
                errors.AddRange(ex.Diagnostics);
            }
        }

        if (sectionName is not null && errors.Count == 0)
            return new PbsSchema(sectionName, keyValuePairs.ToImmutable());

        if (sectionName is null)
        {
            errors.Add(Diagnostic.Create(MissingSectionName, targetType.Locations[0], targetType.Name));
        }

        throw new PbsSchemaException(
            $"Found invalid properties when parsing the schema for type {targetType.Name}",
            errors.ToImmutable()
        );
    }

    private static (bool IsSectionName, string KeyName, PbsSchemaEntry Entry) GetSchemaEntryInfo(
        IPropertySymbol propertySymbol
    )
    {
        var errors = ImmutableArray.CreateBuilder<Diagnostic>();

        var isSectionName = propertySymbol.HasAttribute<PbsSectionNameAttribute>();
        var hasKeyName = propertySymbol.TryGetPbsKeyNameInfo(out var keyNameInfo);
        if (isSectionName && hasKeyName)
        {
            errors.Add(
                Diagnostic.Create(CannotUseKeyNameOnSectionName, propertySymbol.Locations[0], propertySymbol.Name)
            );
        }

        var keyName = hasKeyName ? keyNameInfo.Name : propertySymbol.Name;
        PbsSchemaEntry? entry = null;
        try
        {
            entry = GetSchemaEntry(propertySymbol);
        }
        catch (PbsSchemaException ex)
        {
            errors.AddRange(ex.Diagnostics);
        }

        return errors.Count == 0
            ? (isSectionName, keyName, entry!)
            : throw new PbsSchemaException($"Found errors when processing {propertySymbol}", errors.ToImmutable());
    }

    private static PbsSchemaEntry GetSchemaEntry(IPropertySymbol propertySymbol)
    {
        var underlyingType = GetUnderlyingType(propertySymbol.Type);
        propertySymbol.TryGetPbsTypeInfo(out var typeInfo);

        var fieldTypes = !IsSimpleType(underlyingType)
            ? GetComplexFieldType(underlyingType)
            : GetSimpleFieldType(propertySymbol, underlyingType, typeInfo);

        return new PbsSchemaEntry(propertySymbol, fieldTypes)
        {
            FieldStructure = GetFieldStructure(propertySymbol, typeInfo),
        };
    }

    #region Type helpers
    private static ITypeSymbol GetUnderlyingType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            return arrayTypeSymbol.ElementType;

        if (typeSymbol is not INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
        {
            return typeSymbol;
        }

        if (namedTypeSymbol.ConstructUnboundGenericType().ToDisplayString() == "T?")
        {
            return GetUnderlyingType(namedTypeSymbol.TypeArguments[0]);
        }

        return IsCollectionType(namedTypeSymbol) ? namedTypeSymbol.TypeArguments[0] : namedTypeSymbol;
    }

    private static readonly HashSet<SpecialType> SimpleTypes =
    [
        SpecialType.System_Boolean,
        SpecialType.System_Byte,
        SpecialType.System_SByte,
        SpecialType.System_Int16,
        SpecialType.System_UInt16,
        SpecialType.System_Int32,
        SpecialType.System_UInt32,
        SpecialType.System_Int64,
        SpecialType.System_UInt64,
        SpecialType.System_Single,
        SpecialType.System_Double,
        SpecialType.System_Decimal,
        SpecialType.System_String,
    ];

    private static bool IsSimpleType(ITypeSymbol typeSymbol)
    {
        return typeSymbol.TypeKind == TypeKind.Enum
            || SimpleTypes.Contains(typeSymbol.SpecialType)
            || typeSymbol.ToDisplayString() is GeneratorConstants.Name or GeneratorConstants.Text;
    }

    // Common concrete collection types
    private static readonly ImmutableArray<Type> CollectionTypes =
    [
        typeof(IEnumerable<>),
        typeof(IList<>),
        typeof(ICollection<>),
        typeof(IEnumerable<>),
        typeof(IReadOnlyList<>),
        typeof(IReadOnlyCollection<>),
        typeof(List<>),
        typeof(HashSet<>),
        typeof(LinkedList<>),
        typeof(ImmutableArray<>),
        typeof(ImmutableList<>),
        typeof(ImmutableHashSet<>),
    ];

    private static bool IsCollectionType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol)
            return true;

        if (typeSymbol.SpecialType == SpecialType.System_String)
            return false;

        return typeSymbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol
            && CollectionTypes.Any(x =>
            {
                var fullName = $"{namedTypeSymbol.ContainingNamespace}.{namedTypeSymbol.MetadataName}";
                return x.FullName == fullName;
            });
    }

    #endregion

    private static ImmutableArray<PbsSchemaTypeData> GetSimpleFieldType(
        IPropertySymbol property,
        ITypeSymbol propType,
        PbsTypeInfo? typeAttribute
    )
    {
        if (typeAttribute?.FixedSize > 0 && IsCollectionType(property.Type))
        {
            return
            [
                .. Enumerable
                    .Range(0, typeAttribute.FixedSize)
                    .Select(i =>
                        GetFieldType(property, propType, typeAttribute, typeAttribute.FixedSizeIsMax && i > 0)
                    ),
            ];
        }

        return [GetFieldType(property, propType, typeAttribute)];
    }

    private static PbsSchemaTypeData GetFieldType(
        ISymbol property,
        ITypeSymbol propType,
        PbsTypeInfo? typeAttribute,
        bool isOptional = false
    )
    {
        var propertyType = GetPropertyType(property);
        if (typeAttribute is null)
            return InferFieldType(property, propType, isOptional);

        if (!IsValidFieldType(propType, typeAttribute.FieldType, typeAttribute.EnumType))
        {
            throw new PbsSchemaException(
                $"Property '{property.Name}' has an invalid type. Expected '{typeAttribute.FieldType}' but got '{propType}'.",
                [
                    Diagnostic.Create(
                        PropertyTypeMismatch,
                        property.Locations[0],
                        propType,
                        typeAttribute.FieldType,
                        propertyType
                    ),
                ]
            );
        }

        if (typeAttribute.FieldType is not (PbsFieldType.Enumerable or PbsFieldType.EnumerableOrInteger))
            return new PbsSchemaTypeData(typeAttribute.FieldType, propertyType, isOptional);

        if (typeAttribute.EnumType is null && propType.TypeKind != TypeKind.Enum)
        {
            throw new PbsSchemaException(
                $"Property '{property.Name}' has an enumerable type but no enum type was specified.",
                [Diagnostic.Create(CannotDeduceEnumerableType, property.Locations[0], property.Name)]
            );
        }

        return new PbsSchemaTypeData(
            typeAttribute.FieldType,
            propertyType,
            isOptional,
            typeAttribute.EnumType ?? propType,
            typeAttribute.AllowNone
        );
    }

    private static ImmutableArray<PbsSchemaTypeData> GetComplexFieldType(
        ITypeSymbol propType,
        PbsTypeInfo? typeAttribute = null
    )
    {
        var constructors = propType
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m =>
                m.MethodKind == MethodKind.Constructor
                && m is { DeclaredAccessibility: Accessibility.Public, Parameters.Length: > 0, IsStatic: false }
            )
            .ToImmutableArray();

        var deconstructors = propType
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m =>
                m.Name == "Deconstruct"
                && m
                    is {
                        DeclaredAccessibility: Accessibility.Public,
                        Parameters.Length: > 0,
                        IsStatic: false,
                        ReturnsVoid: true
                    }
                && m.Parameters.All(p => p.RefKind == RefKind.Out)
            )
            .ToImmutableArray();

        var validSerializerPairs = constructors
            .SelectMany(c => deconstructors.Select(d => (Constructor: c, Deconstructor: d)))
            .Where(p =>
                p.Constructor.Parameters.Length == p.Deconstructor.Parameters.Length
                && p.Constructor.Parameters.All(c =>
                    p.Deconstructor.Parameters.Any(d => d.Type.Equals(c.Type, SymbolEqualityComparer.Default))
                )
            )
            .ToImmutableArray();

        if (validSerializerPairs.Length != 1)
        {
            throw new PbsSchemaException(
                $"Type '{propType.Name}' could not find a single constructor-deconstructor pair.",
                [Diagnostic.Create(CannotConstructAndDeconstructType, propType.Locations[0], propType.Name)]
            );
        }

        var constructor = validSerializerPairs[0].Constructor;

        var builder = ImmutableArray.CreateBuilder<PbsSchemaTypeData>(constructor.Parameters.Length);
        builder.AddRange(
            constructor
                .Parameters.Select(constructorParameter => new
                {
                    constructorParameter,
                    parameterType = constructorParameter.Type,
                })
                .Select(t =>
                    GetFieldType(@t.constructorParameter, @t.parameterType, typeAttribute) with
                    {
                        IsOptional = t.constructorParameter.HasExplicitDefaultValue,
                    }
                )
        );

        return builder.ToImmutable();
    }

    private static bool IsValidFieldType(ITypeSymbol propType, PbsFieldType declaredType, ITypeSymbol? enumType)
    {
        return declaredType switch
        {
            PbsFieldType.Integer => propType.SpecialType
                is SpecialType.System_Int32
                    or SpecialType.System_Int16
                    or SpecialType.System_Int64
                    or SpecialType.System_SByte
                    or SpecialType.System_Single
                    or SpecialType.System_Double
                    or SpecialType.System_Decimal,
            PbsFieldType.UnsignedInteger or PbsFieldType.PositiveInteger or PbsFieldType.Hexadecimal =>
                propType.SpecialType
                    is SpecialType.System_Int32
                        or SpecialType.System_Int16
                        or SpecialType.System_Int64
                        or SpecialType.System_SByte
                        or SpecialType.System_UInt32
                        or SpecialType.System_UInt16
                        or SpecialType.System_UInt64
                        or SpecialType.System_Byte
                        or SpecialType.System_Single
                        or SpecialType.System_Double
                        or SpecialType.System_Decimal,
            PbsFieldType.Float => propType.SpecialType
                is SpecialType.System_Single
                    or SpecialType.System_Double
                    or SpecialType.System_Decimal,
            PbsFieldType.Boolean => propType.SpecialType == SpecialType.System_Boolean,
            PbsFieldType.Name or PbsFieldType.String or PbsFieldType.UnformattedText => propType.SpecialType
                == SpecialType.System_String
                || propType.ToDisplayString() is GeneratorConstants.Name or GeneratorConstants.Text,
            PbsFieldType.Symbol => propType.ToDisplayString() is GeneratorConstants.Name,
            PbsFieldType.Enumerable => IsValidEnumType(propType, enumType),
            PbsFieldType.EnumerableOrInteger => IsValidEnumType(propType, enumType)
                || propType.SpecialType
                    is SpecialType.System_Int32
                        or SpecialType.System_Int16
                        or SpecialType.System_Int64
                        or SpecialType.System_SByte
                        or SpecialType.System_Single
                        or SpecialType.System_Double
                        or SpecialType.System_Decimal,
            _ => throw new ArgumentOutOfRangeException(nameof(declaredType), declaredType, null),
        };
    }

    private static bool IsValidEnumType(ITypeSymbol propType, ITypeSymbol? enumType)
    {
        if (enumType is null)
            return propType.TypeKind == TypeKind.Enum;

        var gameDataEntityInterface = enumType.AllInterfaces.FirstOrDefault(i =>
            i.IsGenericType && i.ConstructUnboundGenericType().ToDisplayString() == GeneratorConstants.GameDataEntityT2
        );
        if (gameDataEntityInterface is null)
            return false;

        var keyType = gameDataEntityInterface.TypeArguments[0];
        if (propType.Equals(keyType, SymbolEqualityComparer.Default))
            return true;

        var types = new[] { propType, keyType };
        return types
            .SelectMany(t => t.GetMembers())
            .OfType<IMethodSymbol>()
            .Any(m =>
                m.Name == "op_Implicit"
                && m.Parameters.Length == 1
                && m.ReturnType.Equals(keyType, SymbolEqualityComparer.Default)
                && m.Parameters[0].Type.Equals(propType, SymbolEqualityComparer.Default)
                && m.DeclaredAccessibility == Accessibility.Public
            );
    }

    private static PbsSchemaTypeData InferFieldType(ISymbol property, ITypeSymbol propType, bool isOptional)
    {
        var propertyType = GetPropertyType(property);

        if (propType.TypeKind == TypeKind.Enum)
            return new PbsSchemaTypeData(PbsFieldType.Enumerable, propertyType, isOptional, propType);

        switch (propType.SpecialType)
        {
            case SpecialType.System_Int32
            or SpecialType.System_Int16
            or SpecialType.System_Int64
            or SpecialType.System_SByte:
                return new PbsSchemaTypeData(PbsFieldType.Integer, propertyType, isOptional);
            case SpecialType.System_UInt32
            or SpecialType.System_UInt16
            or SpecialType.System_UInt64
            or SpecialType.System_Byte:
                return new PbsSchemaTypeData(PbsFieldType.UnsignedInteger, propertyType, isOptional);
            case SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal:
                return new PbsSchemaTypeData(PbsFieldType.Float, propertyType, isOptional);
            case SpecialType.System_Boolean:
                return new PbsSchemaTypeData(PbsFieldType.Boolean, propertyType, isOptional);
        }

        var displayString = propType.ToDisplayString();
        if (propType.SpecialType == SpecialType.System_String || displayString == GeneratorConstants.Text)
            return new PbsSchemaTypeData(PbsFieldType.String, propertyType, isOptional);

        return displayString == GeneratorConstants.Name
            ? new PbsSchemaTypeData(PbsFieldType.Symbol, propertyType, isOptional)
            : throw new PbsSchemaException(
                $"Property '{property.Name}' has an invalid type '{propertyType}'.",
                [Diagnostic.Create(InvalidType, property.Locations[0], propertyType)]
            );
    }

    private static ITypeSymbol GetPropertyType(ISymbol property)
    {
        return property switch
        {
            IPropertySymbol p => p.Type,
            IParameterSymbol p => p.Type,
            _ => throw new ArgumentOutOfRangeException(nameof(property), property, null),
        };
    }

    private static PbsFieldStructure GetFieldStructure(IPropertySymbol property, PbsTypeInfo? typeAttribute)
    {
        if (IsCollectionType(property.Type) && (typeAttribute is null || typeAttribute.FixedSize <= 0))
        {
            return property.HasAttribute<PbsKeyRepeatAttribute>()
                ? PbsFieldStructure.Repeating
                : PbsFieldStructure.Array;
        }

        return PbsFieldStructure.Single;
    }
}
