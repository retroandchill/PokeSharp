using System.Collections.Immutable;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Core.Data;
using PokeSharp.SourceGenerator.Model;
using PokeSharp.SourceGenerator.Properties;
using Retro.SourceGeneratorUtilities.Utilities.Members;

namespace PokeSharp.SourceGenerator;

[Generator]
public class GameDataGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor NoIdPropertyDiagnostic = new DiagnosticDescriptor(
        "PSG0001",
        "No Id property found",
        "No Id property found on {0}",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var dataTypes = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                typeof(GameDataEntityAttribute).FullName!,
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
        var info = type.GetAttributes()
            .Where(a => a.AttributeClass?.Name == "GameDataEntityAttribute")
            .Select(a => a.GetGameDataEntityInfo())
            .Single();

        var idProperty = type.GetPublicProperties().SingleOrDefault(p => p.Name == "Id");

        if (idProperty is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(NoIdPropertyDiagnostic, type.Locations[0], type.Name));
            return;
        }

        var templateParameters = new
        {
            Namespace = type.ContainingNamespace.ToDisplayString(),
            ClassName = type.Name,
            ClassType = GetClassType(type),
            Key = idProperty.Type.ToDisplayString(),
            EntityType = info.DataPath is not null ? "Loaded" : "Registered",
            IsReferenceType = !type.IsValueType,
            IsLoaded = info.DataPath is not null,
            info.DataPath,
            Identifiers = type.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m =>
                    m.Name == "AddDefaultValues" && m is { IsStatic: true, Parameters.Length: 0, ReturnsVoid: true }
                )
                .SelectMany(GetIdentifiers)
                .Select(x => new { Identifier = x })
                .ToImmutableArray(),
        };

        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = null;

        context.AddSource(
            $"{type.Name}.g.cs",
            handlebars.Compile(SourceTemplates.GameDataEntityTemplate)(templateParameters)
        );
    }

    private static string GetClassType(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.IsRecord)
        {
            return typeSymbol.IsValueType ? "record struct" : "record";
        }

        return typeSymbol.IsValueType ? "struct" : "class";
    }

    private static IEnumerable<string> GetIdentifiers(IMethodSymbol methodSymbol)
    {
        return methodSymbol
            .DeclaringSyntaxReferences.Select(r => r.GetSyntax())
            .OfType<MethodDeclarationSyntax>()
            .SelectMany(m => m.Body?.Statements ?? [])
            .OfType<ExpressionStatementSyntax>()
            .Where(es =>
                es.Expression
                    is InvocationExpressionSyntax
                    {
                        Expression: IdentifierNameSyntax { Identifier.ValueText: "Register" }
                    }
            )
            .Select(es => (InvocationExpressionSyntax)es.Expression)
            .Where(invocation =>
                invocation.ArgumentList.Arguments.Count > 0
                && invocation.ArgumentList.Arguments[0].Expression
                    is ObjectCreationExpressionSyntax { Initializer: not null }
            )
            .SelectMany(invocation =>
            {
                var objectCreation = (ObjectCreationExpressionSyntax)invocation.ArgumentList.Arguments[0].Expression;
                return objectCreation
                    .Initializer!.Expressions.OfType<AssignmentExpressionSyntax>()
                    .Where(assignment =>
                        assignment.Left is IdentifierNameSyntax { Identifier.ValueText: "Id" }
                        && assignment.Right is LiteralExpressionSyntax literal
                        && literal.Token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StringLiteralToken)
                    )
                    .Select(x => ((LiteralExpressionSyntax)x.Right).Token.ValueText);
            });
    }
}
