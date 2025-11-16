using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TownMapMapper
{
    public static partial TownMap ToGameData(this TownMapInfo dto);

    [MapPropertyFromSource(nameof(MapPoint.Position))]
    [MapPropertyFromSource(nameof(MapPoint.FlyDestination))]
    private static partial MapPoint ToGameData(this MapPointInfo dto);

    public static partial TownMapInfo ToDto(this TownMap entity);

    [MapNestedProperties(nameof(MapPoint.Position))]
    [MapPropertyFromSource(nameof(MapPointInfo.FlyDestinationMap), Use = nameof(MapFlyDestinationMap))]
    [MapPropertyFromSource(nameof(MapPointInfo.FlyDestinationX), Use = nameof(MapFlyDestinationX))]
    [MapPropertyFromSource(nameof(MapPointInfo.FlyDestinationY), Use = nameof(MapFlyDestinationY))]
    private static partial MapPointInfo ToGameData(this MapPoint entity);

    private static Point MapPosition(MapPointInfo mapPoint) => new(mapPoint.X, mapPoint.Y);

    private static FlyDestination? MapFlyDestination(MapPointInfo mapPointInfo)
    {
        return mapPointInfo is { FlyDestinationMap: not null, FlyDestinationX: not null, FlyDestinationY: not null }
            ? new FlyDestination(
                mapPointInfo.FlyDestinationMap.Value,
                new Point(mapPointInfo.FlyDestinationX.Value, mapPointInfo.FlyDestinationY.Value)
            )
            : null;
    }

    private static int? MapFlyDestinationMap(MapPoint mapPointInfo) => mapPointInfo.FlyDestination?.MapId;

    private static int? MapFlyDestinationX(MapPoint mapPointInfo) => mapPointInfo.FlyDestination?.Position.X;

    private static int? MapFlyDestinationY(MapPoint mapPointInfo) => mapPointInfo.FlyDestination?.Position.Y;
}
