using System.Collections.Immutable;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Editor.Core;
using PokeSharp.Editor.SourceGenerator.Formatters;
using PokeSharp.Editor.SourceGenerator.Model;
using PokeSharp.Editor.SourceGenerator.Properties;
using PokeSharp.Editor.SourceGenerator.Utilities;
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
            .Where(t => t is not null)
            .Collect();

        var combined = dataTypes.Combine(context.CompilationProvider);

        context.RegisterSourceOutput(
            combined,
            (ctx, data) =>
            {
                Execute(ctx, data.Left!, data.Right);
            }
        );
    }

    private static void Execute(
        SourceProductionContext context,
        ImmutableArray<INamedTypeSymbol> types,
        Compilation compilation
    )
    {
        // We're going to start with the explicitly defined types, and if we find any complex types in the hierarchy,
        // then we need to explore those types as well.
        var explore = new Queue<EditableTypeInfo>(types.Select(t => t.GetEditableTypeInfo()));
        var explored = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        while (explore.Count > 0)
        {
            var (targetType, name) = explore.Dequeue();
            explored.Add(targetType);

            // We're looking for an edit registry in another assembly, and if we find one, we don't need to generate one.
            var existingRegistry = compilation.GetTypeByMetadataName($"{targetType.ToDisplayString()}EditRegistry");
            if (existingRegistry is not null)
                continue;

            var templateParameters = new
            {
                Namespace = targetType.ContainingNamespace.ToDisplayString(),
                ClassName = targetType.Name,
                Identifier = name ?? targetType.Name,
                Properties = targetType
                    .GetPublicProperties()
                    .Where(x =>
                        x
                            is {
                                IsStatic: false,
                                GetMethod: not null,
                                SetMethod.DeclaredAccessibility: Accessibility.Public
                            }
                    )
                    .Select(x => CreateEditablePropertyInfo(x, explore, explored))
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
    }

    private static EditablePropertyInfo CreateEditablePropertyInfo(
        IPropertySymbol property,
        Queue<EditableTypeInfo> explore,
        HashSet<ITypeSymbol> explored
    )
    {
        switch (property)
        {
            case {
                Type: INamedTypeSymbol { IsGenericType: true, MetadataName: "ImmutableArray`1" } immutableArrayType
            }:
                return new EditablePropertyInfo
                {
                    Name = property.Name,
                    Type = immutableArrayType.ToDisplayString(),
                    PropertyType = PropertyType.List,
                    ValueType = immutableArrayType.TypeArguments[0].ToDisplayString(),
                };
            case { Type: INamedTypeSymbol { IsGenericType: true, MetadataName: "Dictionary`2" } dictionaryType }:
                return new EditablePropertyInfo
                {
                    Name = property.Name,
                    Type = dictionaryType.ToDisplayString(),
                    PropertyType = PropertyType.Dictionary,
                    KeyType = dictionaryType.TypeArguments[0].ToDisplayString(),
                    ValueType = dictionaryType.TypeArguments[1].ToDisplayString(),
                };
            case var _ when !IsSimpleType(property.Type):
                if (!explored.Contains(property.Type))
                {
                    explore.Enqueue(new EditableTypeInfo(property.Type));
                }

                return new EditablePropertyInfo
                {
                    Name = property.Name,
                    Type = property.Type.ToDisplayString(),
                    PropertyType = PropertyType.Object,
                    ObjectType = property.Type.ToDisplayString(NullableFlowState.NotNull),
                };
            default:
                return new EditablePropertyInfo
                {
                    Name = property.Name,
                    Type = property.Type.ToDisplayString(),
                    PropertyType = PropertyType.Scalar,
                };
        }
    }

    private static bool IsSimpleType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.TypeKind == TypeKind.Enum)
        {
            return true;
        }

        if (typeSymbol.ToDisplayString() is GeneratorConstants.Name or GeneratorConstants.Text)
        {
            return true;
        }
        
        return typeSymbol.SpecialType != SpecialType.None;
    }
}
