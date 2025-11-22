using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Core.SourceGenerator.Model;
using PokeSharp.Core.SourceGenerator.Properties;

namespace PokeSharp.Core.SourceGenerator;

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
        var serviceShortcutInfo = type.GetAttributes().GetAutoServiceShortcutInfos().Single();
        var serviceType = type.ToDisplayString();
        var templateParameters = new
        {
            Namespace = type.ContainingNamespace.ToDisplayString(),
            ServiceName = GetClassName(serviceShortcutInfo, type),
            ServiceType = serviceType,
            WrappedType = serviceShortcutInfo.Type switch
            {
                AutoServiceShortcutType.Default => serviceType,
                AutoServiceShortcutType.Options => $"IOptionsMonitor<{serviceType}>",
                AutoServiceShortcutType.GameState => $"IGameStateAccessor<{serviceType}>",
                _ => throw new ArgumentOutOfRangeException(),
            },
            AdditionalAccessor = serviceShortcutInfo.Type switch
            {
                AutoServiceShortcutType.Default => "",
                AutoServiceShortcutType.Options => ".CurrentValue",
                AutoServiceShortcutType.GameState => ".Current",
                _ => throw new ArgumentOutOfRangeException(),
            },
        };

        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = null;

        context.AddSource(
            $"{templateParameters.ServiceName}ServiceShortcut.g.cs",
            handlebars.Compile(SourceTemplates.AutoServiceShortcutTemplate)(templateParameters)
        );
    }

    private static string GetClassName(AutoServiceShortcutInfo serviceShortcutInfo, INamedTypeSymbol typeSymbol)
    {
        if (serviceShortcutInfo.Name is not null)
            return serviceShortcutInfo.Name;

        if (typeSymbol.TypeKind == TypeKind.Interface && typeSymbol.Name[0] == 'I')
        {
            return typeSymbol.Name[1..];
        }

        return typeSymbol.Name;
    }
}
