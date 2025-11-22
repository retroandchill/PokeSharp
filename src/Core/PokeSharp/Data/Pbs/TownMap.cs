using System.Collections.Immutable;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.Core.Data;

namespace PokeSharp.Data.Pbs;

[MessagePackObject(true)]
public readonly record struct Point(int X, int Y);

[MessagePackObject(true)]
public readonly record struct FlyDestination(int MapId, Point Position);

[MessagePackObject(true)]
public readonly record struct MapPoint(
    Point Position,
    Text Name,
    Text? PointOfInterest = null,
    FlyDestination? FlyDestination = null,
    int? Switch = null
);

[GameDataEntity(DataPath = "town_map")]
[MessagePackObject(true)]
public partial class TownMap
{
    public required int Id { get; init; }

    public required Text Name { get; init; }

    public required string Filename { get; init; }

    public required ImmutableArray<MapPoint> Points { get; init; }

    public required ImmutableArray<Name> Flags { get; init; }

    public bool HasFlag(Name flag) => Flags.Contains(flag);
}
