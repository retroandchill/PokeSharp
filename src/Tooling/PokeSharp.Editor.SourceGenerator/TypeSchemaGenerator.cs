using System.Collections.Immutable;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Editor.Core;
using PokeSharp.Editor.SourceGenerator.Model;
using PokeSharp.Editor.SourceGenerator.Properties;
using PokeSharp.Editor.SourceGenerator.Utilities;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;
using Retro.SourceGeneratorUtilities.Utilities.Members;

namespace PokeSharp.Editor.SourceGenerator;

using PropertyTypeResult = (string PropertyClass, string TypeRef, string TypeName, string? KeyType);

[Generator]
public class TypeSchemaGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var dataTypes = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                typeof(EditableTypeAttribute).FullName!,
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

    private static void Execute(SourceProductionContext context, INamedTypeSymbol type)
    {
        var typeInfo = type.GetEditableTypeInfo();

        var templateParameters = new
        {
            Namespace = type.ContainingNamespace.ToDisplayString(),
            ClassName = type.Name,
            Identifier = typeInfo.Name ?? type.Name,
            Properties = type.GetPublicProperties().Select(GetPropertyInfo).ToImmutableArray(),
        };

        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = null;

        context.AddSource(
            $"{templateParameters.ClassName}.g.cs",
            handlebars.Compile(SourceTemplates.EditableEntityTemplate)(templateParameters)
        );
    }

    private static EditablePropertyInfo GetPropertyInfo(IPropertySymbol propertySymbol)
    {
        var (propertyClass, typeRef, valueType, keyType) = GetPropertyTypeInfo(propertySymbol.Type);
        return new EditablePropertyInfo
        {
            Name = propertySymbol.Name,
            PropertyClass = propertyClass,
            TypeRef = typeRef,
            KeyType = keyType,
            ValueType = valueType,
            IsReadOnly = propertySymbol.SetMethod is { IsInitOnly: false },
        };
    }

    private static readonly Dictionary<SpecialType, string> SpecialTypeNames = new()
    {
        [SpecialType.System_Boolean] = "new PrimitiveTypeRef(PrimitiveKind.Bool)",
        [SpecialType.System_SByte] = "new PrimitiveTypeRef(PrimitiveKind.Int8)",
        [SpecialType.System_Int16] = "new PrimitiveTypeRef(PrimitiveKind.Int16)",
        [SpecialType.System_Int32] = "new PrimitiveTypeRef(PrimitiveKind.Int32)",
        [SpecialType.System_Int64] = "new PrimitiveTypeRef(PrimitiveKind.Int64)",
        [SpecialType.System_Byte] = "new PrimitiveTypeRef(PrimitiveKind.Byte)",
        [SpecialType.System_UInt16] = "new PrimitiveTypeRef(PrimitiveKind.UInt16)",
        [SpecialType.System_UInt32] = "new PrimitiveTypeRef(PrimitiveKind.UInt32)",
        [SpecialType.System_UInt64] = "new PrimitiveTypeRef(PrimitiveKind.UInt64)",
        [SpecialType.System_Single] = "new PrimitiveTypeRef(PrimitiveKind.Float)",
        [SpecialType.System_Double] = "new PrimitiveTypeRef(PrimitiveKind.Double)",
        [SpecialType.System_String] = "new PrimitiveTypeRef(PrimitiveKind.String)",
    };

    private static PropertyTypeResult GetPropertyTypeInfo(ITypeSymbol propertyType)
    {
        var propertyName = propertyType.ToDisplayString();
        if (SpecialTypeNames.TryGetValue(propertyType.SpecialType, out var typeRef))
        {
            return ("PrimitiveProperty", typeRef, propertyName, null);
        }

        switch (propertyName)
        {
            case GeneratorConstants.Name:
                return ("PrimitiveProperty", "new PrimitiveTypeRef(PrimitiveKind.Name)", propertyName, null);
            case GeneratorConstants.Text:
                return ("PrimitiveProperty", "new PrimitiveTypeRef(PrimitiveKind.Text)", propertyName, null);
        }

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (propertyType.TypeKind)
        {
            case TypeKind.Enum:
                return (
                    "EnumProperty",
                    $"new EnumTypeRef(typeof({propertyName}), \"{propertyType.Name}\", new Dictionary<string, int>())",
                    propertyName,
                    null
                );
            case TypeKind.Class when propertyType.HasAttribute<EditableTypeAttribute>():
                return ("ObjectProperty", $"new ObjectTypeRef({propertyType}.Type.Name)", propertyName, null);
        }

        if (
            propertyType is INamedTypeSymbol { IsGenericType: true } genericType
            && GetGenericPropertyInfo(genericType, out var propertyTypeInfo)
        )
        {
            return propertyTypeInfo;
        }

        throw new InvalidOperationException(
            $"Could not determine type info for property of type {propertyType.ToDisplayString()}"
        );
    }

    private static bool GetGenericPropertyInfo(INamedTypeSymbol genericType, out PropertyTypeResult propertyTypeInfo)
    {
        switch (genericType.MetadataName)
        {
            case "List`1":
            {
                var (_, innerTypeRef, innerType, _) = GetPropertyTypeInfo(genericType.TypeArguments[0]);
                propertyTypeInfo = ("ListProperty", $"new ListTypeRef({innerTypeRef})", innerType, null);
                return true;
            }
            case "Nullable`1":
            {
                var (_, innerTypeRef, innerType, _) = GetPropertyTypeInfo(genericType.TypeArguments[0]);
                propertyTypeInfo = ("NullableProperty", $"new NullableTypeRef({innerTypeRef})", innerType, null);
                return true;
            }
            case "Dictionary`2":
            {
                var (_, keyTypeRef, keyType, _) = GetPropertyTypeInfo(genericType.TypeArguments[0]);
                var (_, valueTypeRef, valueType, _) = GetPropertyTypeInfo(genericType.TypeArguments[1]);
                propertyTypeInfo = (
                    "NullableProperty",
                    $"new NullableTypeRef({keyTypeRef}, {valueTypeRef})",
                    valueType,
                    keyType
                );
                return true;
            }
        }

        propertyTypeInfo = default;
        return false;
    }
}
