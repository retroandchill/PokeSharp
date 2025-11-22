using System.Collections.Immutable;

namespace PokeSharp.RGSS.Core;

public record Table
{
    public int Dim { get; init; }

    public int X { get; init; }

    public int Y { get; init; }

    public int Z { get; init; }

    public ImmutableArray<int> Data { get; init; } = [];

    public Table(int x)
    {
        Dim = 1;
        X = x;
    }

    public Table(int x, int y)
    {
        Dim = 2;
        X = x;
        Y = y;
    }

    public Table(int x, int y, int z)
    {
        Dim = 3;
        X = x;
        Y = y;
        Z = z;
    }
}
