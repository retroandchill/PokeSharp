using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.SourceGenerator.Attributes;
using PokeSharp.SourceGenerator.Model;
using PokeSharp.SourceGenerator.Properties;

namespace PokeSharp.SourceGenerator;

[Generator]
public class GameServicesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var dataTypes = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                typeof(AutoServiceShortcutAttribute).FullName!,
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
        var templateParameters = new
        {
            Namespace = type.ContainingNamespace.ToDisplayString(),
            ServiceName = GetClassName(type),
            ServiceType = type.ToDisplayString(),
        };

        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = null;

        context.AddSource(
            $"{templateParameters.ServiceName}ServiceShortcut.g.cs",
            handlebars.Compile(SourceTemplates.AutoServiceShortcutTemplate)(templateParameters)
        );
    }

    private static string GetClassName(INamedTypeSymbol typeSymbol)
    {
        var serviceShortcutInfo = typeSymbol.GetAttributes().GetAutoServiceShortcutInfos().Single();

        if (serviceShortcutInfo.Name is not null)
            return serviceShortcutInfo.Name;

        if (typeSymbol.TypeKind == TypeKind.Interface && typeSymbol.Name[0] == 'I')
        {
            return typeSymbol.Name[1..];
        }

        return typeSymbol.Name;
    }
}
