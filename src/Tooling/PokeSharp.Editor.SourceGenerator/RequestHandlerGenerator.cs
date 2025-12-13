using System.Collections.Immutable;
using System.Text.RegularExpressions;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Editor.Core;
using PokeSharp.Editor.SourceGenerator.Model;
using PokeSharp.Editor.SourceGenerator.Properties;
using PokeSharp.Editor.SourceGenerator.Utilities;
using Retro.SourceGeneratorUtilities.Utilities.Types;
using RhoMicro.CodeAnalysis.Library.Extensions;

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

        var routeTree = new Dictionary<PokeEditRequestType, RouteTreeNode>
        {
            [PokeEditRequestType.Get] = new() { Key = "Get", Segment = new SegmentSpec(SegmentType.Literal, "") },
            [PokeEditRequestType.Post] = new() { Key = "Post", Segment = new SegmentSpec(SegmentType.Literal, "") },
            [PokeEditRequestType.Put] = new() { Key = "Put", Segment = new SegmentSpec(SegmentType.Literal, "") },
            [PokeEditRequestType.Patch] = new() { Key = "Patch", Segment = new SegmentSpec(SegmentType.Literal, "") },
            [PokeEditRequestType.Delete] = new() { Key = "Delete", Segment = new SegmentSpec(SegmentType.Literal, "") },
        };
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

            var segments = GetSegments(method, fullRoute);

            var root = routeTree[requestInfo.Type];
            var leafNode = CreateTreeNode(segments.AsSpan(), root);

            var pathParameters = FindPathParameters(segments);
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

            leafNode.Endpoint = new ApiRouteInfo
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
            };
            routes.Add(leafNode.Endpoint);
        }

        var templateParameters = new
        {
            Namespace = controller.ContainingNamespace.ToDisplayString(),
            ClassName = controller.Name,
            Routes = routes.DrainToImmutable(),
            RouteTree = GetAllRouteTreeNodes(routeTree).ToImmutableArray(),
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

    private static readonly Regex PathParamRegex = new(@"\{(\w+)(?::(\w+))?}");

    private static ImmutableArray<SegmentSpec> GetSegments(IMethodSymbol method, string route)
    {
        return
        [
            .. route.Split('/').Select(s => GetSegmentSpec(method, s)).Where(x => x.HasValue).Select(x => x!.Value),
        ];
    }

    private static SegmentSpec? GetSegmentSpec(IMethodSymbol method, string segment)
    {
        var match = PathParamRegex.Match(segment);
        if (!match.Success)
            return new SegmentSpec(SegmentType.Literal, segment);

        var paramName = match.Groups[1].Value;
        var parameter = method.Parameters.FirstOrDefault(p => p.Name == paramName);
        if (parameter is null)
            return null;

        if (
            parameter.Type.SpecialType == SpecialType.System_String
            || parameter.Type.ToDisplayString() == GeneratorConstants.Name
        )
        {
            return new SegmentSpec(SegmentType.String, Parameter: parameter);
        }

        if (parameter.Type.SpecialType == SpecialType.System_Int32)
        {
            return new SegmentSpec(SegmentType.Numeric, parameter.Type.ToDisplayString(), parameter);
        }

        return null;
    }

    private static ImmutableArray<PathParameterInfo> FindPathParameters(ImmutableArray<SegmentSpec> segments)
    {
        var paths = segments
            .Select(x => x.Parameter)
            .OfType<IParameterSymbol>()
            .Select(matchingParameter => new PathParameterInfo(matchingParameter.Type, matchingParameter.Name))
            .ToList();

        return [.. paths.Select((x, i) => x with { Index = i })];
    }

    private static RouteTreeNode CreateTreeNode(ReadOnlySpan<SegmentSpec> spec, RouteTreeNode node)
    {
        while (spec.Length > 0)
        {
            var nextChild = spec[0];
            var key = nextChild.ToSegmentKey();
            if (!node.Children.TryGetValue(key, out var childNode))
            {
                childNode = new RouteTreeNode { Key = key, Segment = nextChild };
                node.Children.Add(key, childNode);
            }

            spec = spec[1..];
            node = childNode;
        }

        return node;
    }

    private readonly record struct RouteTreeData(PokeEditRequestType Type, List<RouteTreeNode> Nodes, string Top);

    private static IEnumerable<RouteTreeData> GetAllRouteTreeNodes(Dictionary<PokeEditRequestType, RouteTreeNode> nodes)
    {
        foreach (var (key, node) in nodes)
        {
            var childList = node.Children.Values.SelectMany(GetAllNodes).ToList();
            if (childList.Count == 0)
                continue;

            yield return new RouteTreeData(key, childList, childList[^1].VariableName);
        }
    }

    private static IEnumerable<RouteTreeNode> GetAllNodes(RouteTreeNode node)
    {
        foreach (var child in node.Children.Values.SelectMany(GetAllNodes))
        {
            yield return child;
        }

        yield return node;
    }
}
