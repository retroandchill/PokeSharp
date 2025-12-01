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
        if (method.Parameters.Length > 2)
        {
            // TODO: Report error
            return;
        }
        var methodName = method.Name.EndsWith("Async") ? method.Name[..^5] : method.Name;

        var requestType = method.Parameters.Length switch
        {
            > 0 when method.Parameters[0].Type.Name != "CancellationToken" => method.Parameters[0].Type,
            _ => null
        };
        var responseType = method.ReturnsVoid ? null : method.ReturnType;
        var hasCancellationToken = method.Parameters.Length > 0 && method.Parameters[^1].Type.Name == "CancellationToken";

        var templateParameters = new
        {
            Namespace = type.ContainingNamespace.ToDisplayString(),
            ClassName = $"{type.Name}{methodName}Handler",
            ServiceClass = type.ToDisplayString(),
            MethodName = methodName,
            RequestName = info.Name ?? methodName,
            RequestType = requestType?.ToDisplayString() ?? "object?",
            RequestTypeNotNull = requestType
                is { IsValueType: false, NullableAnnotation: NullableAnnotation.NotAnnotated },
            ResponseType = responseType?.ToDisplayString() ?? "object?",
            ResponseNotNull = responseType
                is { IsValueType: false, NullableAnnotation: NullableAnnotation.NotAnnotated },
            HasRequestBody = requestType is not null,
            HasResponseBody = !method.ReturnsVoid,
            HasCancellationToken = hasCancellationToken
        };

        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = null;

        context.AddSource(
            $"{templateParameters.ClassName}.g.cs",
            handlebars.Compile(SourceTemplates.RequestHandlerTemplate)(templateParameters)
        );
    }
}
