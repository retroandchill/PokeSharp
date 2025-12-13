using System.Globalization;
using System.Numerics;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IPathSegmentMatcher
{
    int Priority { get; }

    bool TryMatch(int start, ReadOnlySpan<char> pathSegment, out RouteParamSlice? slice);
}

public sealed class LiteralPathSegmentMatcher(string literal) : IPathSegmentMatcher
{
    public int Priority => int.MaxValue;

    public bool TryMatch(int start, ReadOnlySpan<char> pathSegment, out RouteParamSlice? slice)
    {
        slice = null;
        return literal.Equals(pathSegment, StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class NumericPathSegmentMatcher<T> : IPathSegmentMatcher
    where T : struct, INumber<T>
{
    public int Priority => int.MinValue;

    public bool TryMatch(int start, ReadOnlySpan<char> pathSegment, out RouteParamSlice? slice)
    {
        if (T.TryParse(pathSegment, CultureInfo.InvariantCulture, out _))
        {
            slice = new RouteParamSlice(start, pathSegment.Length);
            return true;
        }

        slice = null;
        return false;
    }
}

public sealed class StringPathSegmentMatcher : IPathSegmentMatcher
{
    public int Priority => int.MinValue;

    public bool TryMatch(int start, ReadOnlySpan<char> pathSegment, out RouteParamSlice? slice)
    {
        slice = new RouteParamSlice(start, pathSegment.Length);
        return true;
    }
}
