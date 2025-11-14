using System.Collections.Immutable;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.SourceGenerator.Attributes;
using PokeSharp.SourceGenerator.Model;
using PokeSharp.SourceGenerator.Properties;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;
using Retro.SourceGeneratorUtilities.Utilities.Types;

namespace PokeSharp.SourceGenerator;

[Generator]
public class GameDataRegistrationGenerator : IIncrementalGenerator
{
    public static readonly DiagnosticDescriptor BadInterfacePropertyDiagnostic = new(
        "PSG1001",
        "Registration must be static and read-only",
        "{0} be marked static and read-only",
        "PokeSharp.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var dataTypes = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                typeof(GameDataRegistrationAttribute<>).FullName!,
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
        foreach (var (targetType, priority) in type.GetGameDataRegistrationInfos())
        {
            var fields = type.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(f => f.HasAttribute<GameDataEntityRegistrationAttribute>() && f.Type.IsAssignableTo(targetType))
                .ToArray();

            foreach (var field in fields)
            {
                if (!field.IsReadOnly || !field.IsStatic)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(BadInterfacePropertyDiagnostic, field.Locations[0], field.Name)
                    );
                }
            }

            var templateParameters = new
            {
                Namespace = type.ContainingNamespace.ToDisplayString(),
                Priority = priority,
                ClassName = type.Name,
                EntityType = targetType.ToDisplayString(),
                HasRegistrations = fields.Length > 0,
                Registrations = fields.Select(x => new { x.Name, Type = x.Type.ToDisplayString() }).ToImmutableArray(),
            };

            var handlebars = Handlebars.Create();
            handlebars.Configuration.TextEncoder = null;

            context.AddSource(
                $"{templateParameters.ClassName}.{targetType.Name}.g.cs",
                handlebars.Compile(SourceTemplates.GameDataRegistrationTemplate)(templateParameters)
            );
        }
    }
}
