using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public readonly record struct YieldRangeInfo(
    [property: PbsType(PbsFieldType.UnsignedInteger)] int Min,
    [property: PbsType(PbsFieldType.PositiveInteger)] int Max
);

[PbsData("berry_plants")]
public record BerryPlantInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    [PbsType(PbsFieldType.PositiveInteger)]
    public int HoursPerStage { get; init; } = 3;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int DryingPerHour { get; init; } = 15;

    public YieldRangeInfo Yield { get; init; } = new(2, 5);
}
