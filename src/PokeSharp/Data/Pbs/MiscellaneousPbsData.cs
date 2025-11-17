using System.Collections.Immutable;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.Core.Data;

namespace PokeSharp.Data.Pbs;

public enum MapDirection : byte
{
    North,
    East,
    South,
    West,
}

[MessagePackObject(true)]
public readonly record struct ConnectedMap(int Id, MapDirection Direction, int Offset);

[GameDataEntity(DataPath = "map_connections")]
[MessagePackObject(true)]
public readonly partial record struct MapConnection(int Id, ConnectedMap Map1, ConnectedMap Map2);

[GameDataEntity(DataPath = "regional_dexes")]
[MessagePackObject(true)]
public readonly partial record struct RegionalDex(int Id, ImmutableArray<Name> Entries);

public static class MapDirectionExtensions
{
    public static string ToStringBrief(this MapDirection direction)
    {
        return direction switch
        {
            MapDirection.North => "N",
            MapDirection.East => "E",
            MapDirection.South => "S",
            MapDirection.West => "W",
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
        };
    }
}
