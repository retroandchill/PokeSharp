using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace PokeSharp.Editor.SourceGenerator.Model;

internal readonly record struct PathParameterInfo(ITypeSymbol Type, string Name, int Index = 0);

internal readonly record struct MethodParameter(string Name);

internal record ApiRouteInfo
{
    public required string MethodName { get; init; }

    public required ImmutableArray<PathParameterInfo> PathParameters { get; init; } = [];

    public bool HasPathParameters => PathParameters.Length > 0;

    public required ImmutableArray<MethodParameter> MethodParameters { get; init; } = [];

    public required string RequestType { get; init; }

    public required bool RequestTypeNotNull { get; init; }

    public required string ResponseType { get; init; }

    public required bool ResponseNotNull { get; init; }

    public required bool HasRequestBody { get; init; }

    public required bool HasResponseBody { get; init; }

    public required bool HasCancellationToken { get; init; }
}
