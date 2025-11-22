using System.Collections.Immutable;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.SourceGenerator.Model;
using PokeSharp.SourceGenerator.Properties;
using PokeSharp.UI;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;
using Retro.SourceGeneratorUtilities.Utilities.Types;

namespace PokeSharp.SourceGenerator;

[Generator]
public class MenuOptionRegistrationGenerator : IIncrementalGenerator
{
    public static readonly DiagnosticDescriptor BadInterfacePropertyDiagnostic = new(
        "PSG2001",
        "Registration must be read-only",
        "{0} be marked read-only",
        "PokeSharp.Core.SourceGenerator",
        DiagnosticSeverity.Error,
        true
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var dataTypes = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                typeof(MenuOptionRegistrationAttribute<>).FullName!,
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
        foreach (var (targetType, priority) in type.GetMenuOptionRegistrationInfos())
        {
            var fields = type.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(f => f.HasAttribute<MenuOptionAttribute>() && f.Type.IsAssignableTo(targetType))
                .Select(f => (Field: f, Attribute: f.GetMenuOptionInfo()))
                .ToArray();

            foreach (var (field, _) in fields)
            {
                if (!field.IsReadOnly)
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
                DataType = targetType.ToDisplayString(),
                HasRegistrations = fields.Length > 0,
                Registrations = fields
                    .Select(x => new
                    {
                        x.Field.Name,
                        Id = x.Attribute.Name ?? x.Field.Name,
                        Type = x.Field.Type.ToDisplayString(),
                    })
                    .ToImmutableArray(),
            };

            var handlebars = Handlebars.Create();
            handlebars.Configuration.TextEncoder = null;

            context.AddSource(
                $"{templateParameters.ClassName}.{targetType.Name}.g.cs",
                handlebars.Compile(SourceTemplates.MenuOptionRegistrationTemplate)(templateParameters)
            );
        }
    }
}
