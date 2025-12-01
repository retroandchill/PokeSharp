using System.Collections.Immutable;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Editor.Core;
using PokeSharp.Editor.SourceGenerator.Model;
using PokeSharp.Editor.SourceGenerator.Properties;

namespace PokeSharp.Editor.SourceGenerator;

[Generator]
public class RequestHandlerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var dataTypes = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                typeof(PokeEditRequestAttribute).FullName!,
                (n, _) => n is MethodDeclarationSyntax,
                (ctx, _) =>
                {
                    var type = (MethodDeclarationSyntax)ctx.TargetNode;
                    return ctx.SemanticModel.GetDeclaredSymbol(type) as IMethodSymbol;
                }
            )
            .Where(t => t is not null);

        context.RegisterSourceOutput(dataTypes, Execute!);
    }

    private static void Execute(SourceProductionContext context, IMethodSymbol method)
    {
        var info = method.GetPokeEditRequestInfo();
        var type = method.ContainingType;
        if (method.Parameters.Length > 1)
        {
            // TODO: Report error
            return;
        }

        var requestType = method.Parameters.FirstOrDefault()?.Type;
        var responseType = method.ReturnsVoid ? null : method.ReturnType;

        var templateParameters = new
        {
            Namespace = type.ContainingNamespace.ToDisplayString(),
            ClassName = $"{type.Name}{method.Name}Handler",
            ServiceClass = type.ToDisplayString(),
            MethodName = method.Name,
            RequestName = info.Name ?? method.Name,
            RequestType = requestType?.ToDisplayString() ?? "object?",
            RequestTypeNotNull = requestType
                is { IsValueType: false, NullableAnnotation: NullableAnnotation.NotAnnotated },
            ResponseType = responseType?.ToDisplayString() ?? "object?",
            ResponseNotNull = responseType
                is { IsValueType: false, NullableAnnotation: NullableAnnotation.NotAnnotated },
            HasRequestBody = method.Parameters.Length > 0,
            HasResponseBody = !method.ReturnsVoid,
        };

        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = null;

        context.AddSource(
            $"{templateParameters.ClassName}.g.cs",
            handlebars.Compile(SourceTemplates.RequestHandlerTemplate)(templateParameters)
        );
    }
}
