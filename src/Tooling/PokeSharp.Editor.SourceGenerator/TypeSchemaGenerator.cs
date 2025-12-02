using System.Collections.Immutable;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Editor.Core;
using PokeSharp.Editor.SourceGenerator.Formatters;
using PokeSharp.Editor.SourceGenerator.Model;
using PokeSharp.Editor.SourceGenerator.Properties;
using Retro.SourceGeneratorUtilities.Utilities.Members;

namespace PokeSharp.Editor.SourceGenerator;

[Generator]
public class TypeSchemaGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var dataTypes = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                typeof(EditableTypeAttribute<>).FullName!,
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
        var (targetType, name) = type.GetEditableTypeInfo();

        var templateParameters = new
        {
            Namespace = type.ContainingNamespace.ToDisplayString(),
            ClassName = type.Name,
            TargetType = targetType.ToDisplayString(),
            TargetTypeName = targetType.Name,
            Identifier = name ?? targetType.Name,
            Properties = targetType
                .GetPublicProperties()
                .Where(x =>
                    x is { IsStatic: false, GetMethod: not null, SetMethod.DeclaredAccessibility: Accessibility.Public }
                )
                .Select(CreateEditablePropertyInfo)
                .ToImmutableArray(),
        };

        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = null;
        handlebars.Configuration.FormatterProviders.Add(new EnumStringFormatter());

        context.AddSource(
            $"{templateParameters.ClassName}.g.cs",
            handlebars.Compile(SourceTemplates.EditableEntityTemplate)(templateParameters)
        );
    }

    private static EditablePropertyInfo CreateEditablePropertyInfo(IPropertySymbol property)
    {
        return property switch
        {
            { Type: INamedTypeSymbol { IsGenericType: true, MetadataName: "ImmutableArray`1" } immutableArrayType } =>
                new EditablePropertyInfo
                {
                    Name = property.Name,
                    Type = immutableArrayType.ToDisplayString(),
                    PropertyType = PropertyType.List,
                    ValueType = immutableArrayType.TypeArguments[0].ToDisplayString(),
                },
            { Type: INamedTypeSymbol { IsGenericType: true, MetadataName: "Dictionary`2" } dictionaryType } =>
                new EditablePropertyInfo()
                {
                    Name = property.Name,
                    Type = dictionaryType.ToDisplayString(),
                    PropertyType = PropertyType.Dictionary,
                    KeyType = dictionaryType.TypeArguments[0].ToDisplayString(),
                    ValueType = dictionaryType.TypeArguments[1].ToDisplayString(),
                },
            _ => new EditablePropertyInfo
            {
                Name = property.Name,
                Type = property.Type.ToDisplayString(),
                PropertyType = PropertyType.Scalar,
            },
        };
    }
}
