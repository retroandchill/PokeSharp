namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public readonly record struct RouteParamSlice(int Start, int Length);

public ref struct RouteValueBuffer(ReadOnlySpan<char> route, Span<RouteParamSlice> storage)
{
    private readonly ReadOnlySpan<char> _route = route;
    private readonly Span<RouteParamSlice> _items = storage;
    public int Count { get; private set; } = 0;

    public ReadOnlySpan<char> this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Count - 1);
            var slice = _items[index];
            return _route.Slice(slice.Start, slice.Length);
        }
    }

    public void Add(RouteParamSlice slice)
    {
        if (Count == _items.Length)
        {
            throw new InvalidOperationException("Buffer is full.");
        }

        _items[Count++] = slice;
    }
}
