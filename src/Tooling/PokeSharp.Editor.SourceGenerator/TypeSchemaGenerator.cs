using System.Collections.Immutable;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Editor.SourceGenerator.Formatters;
using PokeSharp.Editor.SourceGenerator.Model;
using PokeSharp.Editor.SourceGenerator.Properties;
using PokeSharp.Editor.SourceGenerator.Utilities;

namespace PokeSharp.Editor.SourceGenerator;

[Generator]
public class TypeSchemaGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var forCalls = context
            .SyntaxProvider.CreateSyntaxProvider(
                (node, _) => node is InvocationExpressionSyntax,
                (ctx, _) => GetForCallInfo(ctx)
            )
            .Where(info => info is not null)
            .Select((info, _) => info!.Value)
            .Collect();

        var combined = forCalls.Combine(context.CompilationProvider);

        context.RegisterSourceOutput(
            combined,
            (ctx, data) =>
            {
                Execute(ctx, data.Left, data.Right);
            }
        );
    }

    private static ForCallInfo? GetForCallInfo(GeneratorSyntaxContext ctx)
    {
        if (ctx.Node is not InvocationExpressionSyntax invocation)
            return null;

        if (ctx.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol { Name: "For" } symbol)
            return null;

        // Ensure it's your EditorModelBuilder.For<T>
        var editorModelBuilderType = ctx.SemanticModel.Compilation.GetTypeByMetadataName(
            "PokeSharp.Editor.Core.PokeEdit.Properties.EditorModelBuilder"
        );

        if (!SymbolEqualityComparer.Default.Equals(symbol.ContainingType, editorModelBuilderType))
            return null;

        // Get the type argument T
        if (symbol.TypeArguments.Length != 1)
            return null;

        if (symbol.TypeArguments[0] is not INamedTypeSymbol t)
            return null;

        var lambda = invocation
            .ArgumentList.Arguments.Select(a => a.Expression)
            .OfType<LambdaExpressionSyntax>()
            .FirstOrDefault();

        if (lambda is null)
            return null;

        var location = ctx.SemanticModel.GetInterceptableLocation(invocation);
        if (location is null)
        {
            return null;
        }

        return new ForCallInfo(t, lambda, location.Version, location.Data);
    }

    private readonly record struct ForCallInfo(
        INamedTypeSymbol TargetType,
        LambdaExpressionSyntax Lambda,
        int Version,
        string Data
    );

    private static void Execute(
        SourceProductionContext context,
        ImmutableArray<ForCallInfo> types,
        Compilation compilation
    )
    {
        var forCallInfos = new Dictionary<INamedTypeSymbol, List<ForCallInfo>>(SymbolEqualityComparer.Default);
        foreach (var info in types)
        {
            if (!forCallInfos.TryGetValue(info.TargetType, out var list))
            {
                list = [];
                forCallInfos.Add(info.TargetType, list);
            }

            list.Add(info);
        }

        // We're going to start with the explicitly defined types, and if we find any complex types in the hierarchy,
        // then we need to explore those types as well.
        var explore = new Queue<INamedTypeSymbol>(types.Select(x => x.TargetType));
        var explored = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        while (explore.Count > 0)
        {
            var targetType = explore.Dequeue();
            explored.Add(targetType);

            var forCalls = forCallInfos.TryGetValue(targetType, out var infos) ? infos : [];

            var properties = targetType
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x =>
                    x
                        is {
                            IsStatic: false,
                            GetMethod.DeclaredAccessibility: Accessibility.Public,
                            SetMethod.DeclaredAccessibility: Accessibility.Public
                        }
                )
                .Select(x => CreateEditablePropertyInfo(x, explore, explored, forCalls, compilation))
                .OfType<EditablePropertyInfo>()
                .ToImmutableArray();

            var templateParameters = new
            {
                Namespace = targetType.ContainingNamespace.ToDisplayString(),
                ClassName = targetType.Name,
                Identifier = $"\"{targetType.Name}\"",
                Properties = properties.SetItem(properties.Length - 1, properties[^1] with { IsLast = true }),
                HasForCalls = forCalls.Count > 0,
                ForCalls = forCalls,
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

    private static EditablePropertyInfo? CreateEditablePropertyInfo(
        IPropertySymbol property,
        Queue<INamedTypeSymbol> explore,
        HashSet<ITypeSymbol> explored,
        IReadOnlyList<ForCallInfo> forCalls,
        Compilation compilation
    )
    {
        var displayName = $"\"{property.Name}\"";
        var defaultValue = GetDefaultValue(property);
        foreach (var (invocation, semanticModel) in forCalls.SelectMany(x => GetPropertyCalls(x.Lambda, compilation)))
        {
            var symbol = semanticModel.GetSymbolInfo(invocation).Symbol;

            if (symbol is not IMethodSymbol methodSymbol)
                continue;

            switch (methodSymbol.Name)
            {
                case "Ignore":
                    return null;
                case "DisplayName":
                    displayName = invocation.ArgumentList.Arguments[0].Expression.ToString();
                    break;
                case "DefaultValue":
                    defaultValue = invocation.ArgumentList.Arguments[0].Expression.ToString();
                    break;
            }
        }

        switch (property)
        {
            case {
                Type: INamedTypeSymbol { IsGenericType: true, MetadataName: "ImmutableArray`1" } immutableArrayType
            }:
                return new EditablePropertyInfo
                {
                    Name = property.Name,
                    DisplayName = displayName,
                    Type = immutableArrayType.ToDisplayString(),
                    PropertyType = PropertyType.List,
                    ValueType = immutableArrayType.TypeArguments[0].ToDisplayString(),
                    DefaultValue = defaultValue,
                };
            case {
                Type: INamedTypeSymbol { IsGenericType: true, MetadataName: "ImmutableDictionary`2" } dictionaryType
            }:
                return new EditablePropertyInfo
                {
                    Name = property.Name,
                    DisplayName = displayName,
                    Type = dictionaryType.ToDisplayString(),
                    PropertyType = PropertyType.Dictionary,
                    KeyType = dictionaryType.TypeArguments[0].ToDisplayString(),
                    ValueType = dictionaryType.TypeArguments[1].ToDisplayString(),
                    DefaultValue = defaultValue,
                };
            case { Type: INamedTypeSymbol complexType } when !IsSimpleType(complexType):
                if (!explored.Contains(complexType))
                {
                    explore.Enqueue(complexType);
                }

                return new EditablePropertyInfo
                {
                    Name = property.Name,
                    DisplayName = displayName,
                    Type = property.Type.ToDisplayString(),
                    PropertyType = PropertyType.Object,
                    ObjectType = property.Type.ToDisplayString(NullableFlowState.NotNull),
                    DefaultValue = defaultValue,
                };
            default:
                return new EditablePropertyInfo
                {
                    Name = property.Name,
                    DisplayName = displayName,
                    Type = property.Type.ToDisplayString(),
                    PropertyType = PropertyType.Scalar,
                    DefaultValue = defaultValue,
                };
        }
    }

    private static IEnumerable<(InvocationExpressionSyntax Invocation, SemanticModel semanticModel)> GetPropertyCalls(
        LambdaExpressionSyntax lambda,
        Compilation compilation
    )
    {
        return [];
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

        return typeSymbol.SpecialType
            is SpecialType.System_Boolean
                or SpecialType.System_String
                or SpecialType.System_SByte
                or SpecialType.System_Int16
                or SpecialType.System_Int32
                or SpecialType.System_Int64
                or SpecialType.System_Byte
                or SpecialType.System_UInt16
                or SpecialType.System_UInt32
                or SpecialType.System_UInt64
                or SpecialType.System_Single
                or SpecialType.System_Double;
    }

    private static string GetDefaultValue(IPropertySymbol propertySymbol)
    {
        var syntax = propertySymbol
            .DeclaringSyntaxReferences.Select(x => x.GetSyntax())
            .OfType<PropertyDeclarationSyntax>()
            .Select(x => x.Initializer)
            .FirstOrDefault(x => x is not null);
        if (syntax is not null)
        {
            return syntax.Value.ToString();
        }

        return propertySymbol.Type switch
        {
            INamedTypeSymbol { IsGenericType: true, MetadataName: "ImmutableArray`1" } => "[]",
            INamedTypeSymbol { IsGenericType: true, MetadataName: "ImmutableDictionary`2" } dictionaryType =>
                $"{dictionaryType}.Empty",
            { IsValueType: false } or INamedTypeSymbol { IsGenericType: true, MetadataName: "Nullable`1" } => "null",
            { SpecialType: SpecialType.System_Boolean } => "false",
            { SpecialType: SpecialType.System_String } => "string.Empty",
            {
                SpecialType: SpecialType.System_SByte
                    or SpecialType.System_Int16
                    or SpecialType.System_Int32
                    or SpecialType.System_Int64
                    or SpecialType.System_Byte
                    or SpecialType.System_UInt16
                    or SpecialType.System_UInt32
                    or SpecialType.System_UInt64
                    or SpecialType.System_Single
                    or SpecialType.System_Double
            } => "0",
            _ when propertySymbol.Type.ToDisplayString() is GeneratorConstants.Name or GeneratorConstants.Text =>
                $"{propertySymbol.Type.ToDisplayString()}.None",
            _ => "default",
        };
    }
}
