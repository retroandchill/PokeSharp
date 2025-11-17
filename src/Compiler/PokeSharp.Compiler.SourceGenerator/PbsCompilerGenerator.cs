using System.Collections.Immutable;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.SourceGenerator.Exceptions;
using PokeSharp.Compiler.SourceGenerator.Model;
using PokeSharp.Compiler.SourceGenerator.Properties;
using PokeSharp.Compiler.SourceGenerator.Properties;
using PokeSharp.Compiler.SourceGenerator.Properties;
using PokeSharp.Compiler.SourceGenerator.Utilities;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;
using Retro.SourceGeneratorUtilities.Utilities.Members;

namespace PokeSharp.Compiler.SourceGenerator;

[Generator]
public class PbsCompilerGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor InvalidPbsType = new(
        "PBS0001",
        "Invalid PBS model type",
        "{0} must be either a class or a struct, and cannot be marked abstract",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor MissingSectionName = new(
        "PBS0002",
        "Missing section name property",
        "{0} does not have a property marked with [PbsSectionName], exactly one property must be marked with this attribute",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor CannotUseKeyNameOnSectionName = new(
        "PBS0003",
        "Cannot use [PbsKeyName] on section name property",
        "{0} cannot be markeed with [PbsKeyName] because it is also marked with [PbsSectionName]",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor InvalidType = new(
        "PBS0004",
        "Property type cannot be deserialized",
        "Type {0} cannot be deserialized",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor PropertyTypeMismatch = new(
        "PBS0005",
        "Property type specified not valid",
        "Type {0} has been specified as {1}, which is not valid for type {2}",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor CannotDeduceEnumerableType = new(
        "PBS0006",
        "Cannot deduce enumerable type for property",
        "Property {0} has been specified as enumerable, but no enum type was specified, and it is not of type enum",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor CannotConstructAndDeconstructType = new(
        "PBS0007",
        "Type requires exactly one constructor and one Deconstruct method with the same parameters",
        "Type {0} requires exactly one constructor and one Deconstruct method with the same parameters",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor MultipleSectionNames = new(
        "PBS0008",
        "Type requires exactly one section name property",
        "Specified property {0} as [PbsSectionName], but there are multiple properties marked with this attribute",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor UnknownSourceGenerationError = new(
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
            .Where(t => t is not null)
            .Collect();

        context.RegisterSourceOutput(dataTypes, Execute!);
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<INamedTypeSymbol> targetTypes)
    {
        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = null;
        handlebars.RegisterTemplate("ParseLogic", SourceTemplates.ParseLogicTemplate);
        handlebars.RegisterTemplate("ParseMethodCall", SourceTemplates.ParseMethodCallTemplate);
        handlebars.RegisterTemplate("EvaluateComplexType", SourceTemplates.EvaluateComplexTypeTemplate);
        handlebars.RegisterTemplate("WriteMethodCall", SourceTemplates.WriteMethodCallTemplate);
        handlebars.RegisterTemplate("NullableAccess", SourceTemplates.NullableAccessTemplate);
        handlebars.RegisterTemplate("PropertyWrite", SourceTemplates.PropertyWriteTemplate);
        handlebars.RegisterTemplate("PropertyWriteChecks", SourceTemplates.PropertyWriteChecksTemplate);

        handlebars.RegisterHelper(
            "WithIndent",
            (writer, options, _, parameters) =>
            {
                var indent = parameters[0] as string ?? "";

                // Capture the block content
                var content = options.Template();

                // Split the content into lines
                var lines = content.Split('\n');

                // Add indentation to each line except empty lines
                var indentedLines = lines.Select(line => string.IsNullOrWhiteSpace(line) ? line : indent + line);

                // Join the lines back together
                writer.WriteSafeString(string.Join("\n", indentedLines));
            }
        );

        var sourceTemplate = handlebars.Compile(SourceTemplates.PbsSerializerTemplate);

        handlebars.RegisterHelper(
            "Indexed",
            (writer, options, ctx, _) =>
            {
                var data = (PbsSchemaTypeData)ctx.Value;
                var content = options.Template();
                writer.WriteSafeString(content.Replace("{{Index}}", data.Index.ToString()));
            }
        );

        foreach (var targetType in targetTypes)
        {
            try
            {
                var schema = BuildSchema(targetType);
                context.AddSource($"{schema.ClassName}.g.cs", sourceTemplate(schema));
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
    }

    private static PbsSchema BuildSchema(INamedTypeSymbol targetType)
    {
        var info = targetType.GetPbsDataInfo();
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

        return BuildSchema(targetType, info);
    }

    private static PbsSchema BuildSchema(ITypeSymbol targetType, PbsDataInfo info, bool isSubSchema = false)
    {
        var errors = ImmutableArray.CreateBuilder<Diagnostic>();
        PbsSchemaEntry? sectionName = null;
        var keyValuePairs = ImmutableArray.CreateBuilder<PbsSchemaEntry>();
        var subSchemas = ImmutableArray.CreateBuilder<PbsSubSchema>();

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
                if (!property.HasAttribute<PbsSubSchemaAttribute>())
                {
                    var entry = GetSchemaEntryInfo(property);
                    keyValuePairs.Add(entry);
                    if (!entry.IsSectionName)
                        continue;

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
                    var underlyingType = GetUnderlyingType(property.Type);
                    var infoData = new PbsDataInfo(string.Empty);
                    var schemaEntry = BuildSchema(underlyingType, infoData, true);
                    subSchemas.Add(
                        new PbsSubSchema(property, schemaEntry) { IsCollection = IsCollectionType(property.Type) }
                    );
                }
            }
            catch (PbsSchemaException ex)
            {
                errors.AddRange(ex.Diagnostics);
            }
        }

        if (sectionName is not null && errors.Count == 0)
            return new PbsSchema(
                targetType,
                keyValuePairs.ToImmutable(),
                info.BaseFilename,
                info.IsOptional,
                info.ComparisonFactory
            )
            {
                SubSchemas = subSchemas.ToImmutable(),
            };

        if (sectionName is null)
        {
            errors.Add(Diagnostic.Create(MissingSectionName, targetType.Locations[0], targetType.Name));
        }

        throw new PbsSchemaException(
            $"Found invalid properties when parsing the schema for type {targetType.Name}",
            errors.ToImmutable()
        );
    }

    private static PbsSchemaEntry GetSchemaEntryInfo(IPropertySymbol propertySymbol)
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
            entry = GetSchemaEntry(keyName, propertySymbol, isSectionName);
        }
        catch (PbsSchemaException ex)
        {
            errors.AddRange(ex.Diagnostics);
        }

        return errors.Count == 0
            ? entry!
            : throw new PbsSchemaException($"Found errors when processing {propertySymbol}", errors.ToImmutable());
    }

    private static PbsSchemaEntry GetSchemaEntry(string keyName, IPropertySymbol propertySymbol, bool isSectionName)
    {
        var underlyingType = GetUnderlyingType(propertySymbol.Type);
        propertySymbol.TryGetPbsTypeInfo(out var typeInfo);

        var fieldTypes = !IsSimpleType(underlyingType)
            ? GetComplexFieldType(underlyingType)
            : GetSimpleFieldType(propertySymbol, underlyingType, typeInfo);

        return new PbsSchemaEntry(keyName, propertySymbol, fieldTypes)
        {
            IsSectionName = isSectionName,
            IsFixedSize = typeInfo?.FixedSize > 0,
            FieldStructure = GetFieldStructure(propertySymbol, typeInfo),
            CustomValidateMethod = propertySymbol.TryGetPbsWriteValidationInfo(out var info) ? info.MethodName : null,
            CustomWriteMethod = propertySymbol.TryGetPbsCustomWriteInfo(out var customWriteInfo)
                ? customWriteInfo.MethodName
                : null,
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
                        GetFieldType(
                            property,
                            propType,
                            typeAttribute,
                            i,
                            typeAttribute.FixedSizeIsMax && i > 0 ? "0" : null
                        ) with
                        {
                            IsLast = i == typeAttribute.FixedSize - 1,
                        }
                    ),
            ];
        }

        return [GetFieldType(property, propType, typeAttribute, 0)];
    }

    private static PbsSchemaTypeData GetFieldType(
        ISymbol property,
        ITypeSymbol propType,
        PbsTypeInfo? typeAttribute,
        int index,
        string? defaultValue = null
    )
    {
        if (typeAttribute is null)
            return InferFieldType(property, propType, index, defaultValue);

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
                        propType
                    ),
                ]
            );
        }

        if (typeAttribute.FieldType is not (PbsFieldType.Enumerable or PbsFieldType.EnumerableOrInteger))
            return new PbsSchemaTypeData(typeAttribute.FieldType, propType)
            {
                Index = index,
                DefaultValue = defaultValue,
            };

        if (typeAttribute.EnumType is null && propType.TypeKind != TypeKind.Enum)
        {
            throw new PbsSchemaException(
                $"Property '{property.Name}' has an enumerable type but no enum type was specified.",
                [Diagnostic.Create(CannotDeduceEnumerableType, property.Locations[0], property.Name)]
            );
        }

        return new PbsSchemaTypeData(typeAttribute.FieldType, propType, typeAttribute.EnumType ?? propType)
        {
            Index = index,
            DefaultValue = defaultValue,
            AllowNone = typeAttribute.AllowNone,
        };
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
                    ConstructorParameter = constructorParameter,
                    ParameterType = constructorParameter.Type,
                })
                .Select(
                    (t, i) =>
                        GetFieldType(t.ConstructorParameter, GetUnderlyingType(t.ParameterType), typeAttribute, i) with
                        {
                            DefaultValue = t.ConstructorParameter.GetDefaultValueString(),
                            IsLast = i == constructor.Parameters.Length - 1,
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

    private static PbsSchemaTypeData InferFieldType(
        ISymbol property,
        ITypeSymbol propType,
        int index,
        string? defaultValue
    )
    {
        var propertyType = GetPropertyType(property);

        if (propType.TypeKind == TypeKind.Enum)
            return new PbsSchemaTypeData(PbsFieldType.Enumerable, propertyType, propType)
            {
                Index = index,
                DefaultValue = defaultValue,
            };

        switch (propType.SpecialType)
        {
            case SpecialType.System_Int32
            or SpecialType.System_Int16
            or SpecialType.System_Int64
            or SpecialType.System_SByte:
                return new PbsSchemaTypeData(PbsFieldType.Integer, propertyType)
                {
                    Index = index,
                    DefaultValue = defaultValue,
                };
            case SpecialType.System_UInt32
            or SpecialType.System_UInt16
            or SpecialType.System_UInt64
            or SpecialType.System_Byte:
                return new PbsSchemaTypeData(PbsFieldType.UnsignedInteger, propertyType)
                {
                    Index = index,
                    DefaultValue = defaultValue,
                };
            case SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal:
                return new PbsSchemaTypeData(PbsFieldType.Float, propertyType)
                {
                    Index = index,
                    DefaultValue = defaultValue,
                };
            case SpecialType.System_Boolean:
                return new PbsSchemaTypeData(PbsFieldType.Boolean, propertyType)
                {
                    Index = index,
                    DefaultValue = defaultValue,
                };
        }

        var displayString = propType.ToDisplayString();
        if (propType.SpecialType == SpecialType.System_String || displayString == GeneratorConstants.Text)
            return new PbsSchemaTypeData(PbsFieldType.String, propertyType)
            {
                Index = index,
                DefaultValue = defaultValue,
            };

        return displayString == GeneratorConstants.Name
            ? new PbsSchemaTypeData(PbsFieldType.Symbol, propertyType) { Index = index, DefaultValue = defaultValue }
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
