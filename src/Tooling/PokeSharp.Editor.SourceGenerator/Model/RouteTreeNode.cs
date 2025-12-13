using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace PokeSharp.Editor.SourceGenerator.Model;

internal enum SegmentType : byte
{
    Literal,
    Numeric,
    String,
}

internal readonly record struct SegmentSpec(SegmentType Type, string? Value = null, IParameterSymbol? Parameter = null)
{
    public string MatcherName
    {
        get
        {
            return Type switch
            {
                SegmentType.Literal => $"LiteralPathSegmentMatcher(@\"{Value}\")",
                SegmentType.Numeric => $"NumericPathSegmentMatcher<{Value}>()",
                SegmentType.String => "StringPathSegmentMatcher()",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }

    public string ToSegmentKey()
    {
        return Type switch
        {
            SegmentType.Literal => $"lit:{Value}",
            SegmentType.Numeric => $"num:{Value}",
            SegmentType.String => "str",
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}

internal class RouteTreeNode
{
    public required string Key { get; init; } // identity (literal/param)
    public string VariableName => Key.Replace(":", "_");
    public required SegmentSpec Segment { get; init; } // info to emit matcher
    public Dictionary<string, RouteTreeNode> Children { get; } = new();
    public ApiRouteInfo? Endpoint { get; set; }
    public bool HasEndpoint => Endpoint is not null;
}
