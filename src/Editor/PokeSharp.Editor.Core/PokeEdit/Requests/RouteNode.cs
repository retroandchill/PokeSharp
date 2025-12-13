using System.Collections.Immutable;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public record RouteNode
{
    public required IPathSegmentMatcher Matcher { get; init; }

    public IRequestHandler? Handler { get; init; }

    public ImmutableArray<RouteNode> Children { get; init; } = [];
}
