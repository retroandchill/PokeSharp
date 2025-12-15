using System.Collections.Immutable;
using System.Reflection;
using System.Text.RegularExpressions;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Editor.Core;
using PokeSharp.Editor.SourceGenerator.Model;
using PokeSharp.Editor.SourceGenerator.Properties;
using PokeSharp.Editor.SourceGenerator.Utilities;
using Retro.SourceGeneratorUtilities.Utilities.Types;

namespace PokeSharp.Editor.SourceGenerator;

[Generator]
public class RequestHandlerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var dataTypes = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                typeof(PokeEditControllerAttribute).FullName!,
                (n, _) => n is ClassDeclarationSyntax,
                (ctx, _) =>
                {
                    var type = (ClassDeclarationSyntax)ctx.TargetNode;
                    return ctx.SemanticModel.GetDeclaredSymbol(type) as INamedTypeSymbol;
                }
            )
            .Where(t => t is not null);

        context.RegisterSourceOutput(dataTypes, Execute!);
    }

    private static void Execute(SourceProductionContext context, INamedTypeSymbol controller)
    {
        var info = controller.GetPokeEditControllerInfo();

        var routes = ImmutableArray.CreateBuilder<ApiRouteInfo>();
        foreach (
            var (method, requestInfo) in controller
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public && !m.IsStatic)
                .SelectMany(x => x.GetPokeEditRequestInfos(), (m, i) => (Method: m, Info: i))
        )
        {
            string fullRoute;
            if (info.Path is null)
            {
                if (requestInfo.Route is null)
                {
                    // TODO: Report error
                    continue;
                }

                fullRoute = requestInfo.Route;
            }
            else
            {
                fullRoute = requestInfo.Route is null ? info.Path : $"{info.Path}/{requestInfo.Route}";
            }

            var pathParameters = FindPathParameters(method, fullRoute);
            var nonPathParameters = method
                .Parameters.Where(p => !pathParameters.Any(x => x.Name == p.Name) && p.Type.Name != "CancellationToken")
                .ToImmutableArray();

            if (nonPathParameters.Length > 1)
            {
                // TODO: Emit diagnostic
                continue;
            }

            var methodName = method.Name.EndsWith("Async") ? method.Name[..^5] : method.Name;

            var requestType = nonPathParameters.Length > 0 ? nonPathParameters[0].Type : null;
            var responseType = method.ReturnsVoid ? null : method.ReturnType;
            var hasCancellationToken =
                method.Parameters.Length > 0 && method.Parameters[^1].Type.Name == "CancellationToken";

            routes.Add(
                new ApiRouteInfo
                {
                    MethodName = methodName,
                    PathParameters = pathParameters,
                    MethodParameters =
                    [
                        .. method.Parameters.Select(p =>
                        {
                            if (p.Type.IsAssignableTo<CancellationToken>())
                            {
                                return new MethodParameter("cancellationToken");
                            }

                            return pathParameters.Any(x => x.Name == p.Name)
                                ? new MethodParameter(p.Name)
                                : new MethodParameter("request");
                        }),
                    ],
                    RequestType = requestType?.ToDisplayString() ?? "object?",
                    RequestTypeNotNull =
                        requestType is { IsValueType: false, NullableAnnotation: NullableAnnotation.NotAnnotated },
                    ResponseType = responseType?.ToDisplayString() ?? "object?",
                    ResponseNotNull =
                        responseType is { IsValueType: false, NullableAnnotation: NullableAnnotation.NotAnnotated },
                    HasRequestBody = requestType is not null,
                    HasResponseBody =
                        method is { ReturnsVoid: false, ReturnType: INamedTypeSymbol { IsGenericType: true } },
                    HasCancellationToken = hasCancellationToken,
                }
            );
        }

        var templateParameters = new
        {
            Namespace = controller.ContainingNamespace.ToDisplayString(),
            ClassName = controller.Name,
            Routes = routes.DrainToImmutable(),
        };

        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = null;

        handlebars.RegisterHelper(
            "PathParamGetter",
            (writer, ctx, _) =>
            {
                var bufferAccess = $"buffer[{ctx[nameof(PathParameterInfo.Index)]}]";
                if (ctx[nameof(PathParameterInfo.Type)] is not ITypeSymbol typeSymbol)
                {
                    return;
                }

                if (typeSymbol.SpecialType == SpecialType.System_String)
                {
                    writer.WriteSafeString($"{bufferAccess}.ToString()");
                }
                else if (typeSymbol.ToDisplayString() == GeneratorConstants.Name)
                {
                    writer.WriteSafeString($"new Name({bufferAccess})");
                }
                else
                {
                    writer.WriteSafeString($"{typeSymbol.ToDisplayString()}.Parse({bufferAccess})");
                }
            }
        );

        handlebars.RegisterHelper(
            "RequestParameters",
            (writer, ctx, args) =>
            {
                if (ctx[nameof(ApiRouteInfo.MethodParameters)] is not ImmutableArray<MethodParameter> parameters)
                {
                    return;
                }

                var isAsync = args[0].ToString() == "true";
                var hasCancellationToken = ctx[nameof(ApiRouteInfo.HasCancellationToken)].ToString() == "true";

                writer.WriteSafeString(
                    string.Join(
                        ", ",
                        parameters
                            .Select(x => x.Name)
                            .Where(x => x != "cancellationToken" || (isAsync && hasCancellationToken))
                    )
                );
            }
        );

        context.AddSource(
            $"{templateParameters.ClassName}.g.cs",
            handlebars.Compile(SourceTemplates.RequestHandlerTemplate)(templateParameters)
        );
    }

    private static readonly Regex PathParamRegex = new(@"\{(\w+)}");

    private static ImmutableArray<PathParameterInfo> FindPathParameters(IMethodSymbol method, string baseRoute)
    {
        var paths = baseRoute
            .Split('/')
            .Select(segment => PathParamRegex.Match(segment))
            .Where(match => match.Success)
            .Select(match => method.Parameters.FirstOrDefault(p => p.Name == match.Groups[1].Value))
            .OfType<IParameterSymbol>()
            .Select(matchingParameter => new PathParameterInfo(matchingParameter.Type, matchingParameter.Name))
            .ToList();

        return [.. paths.Select((x, i) => x with { Index = i })];
    }
}
