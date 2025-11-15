using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public readonly record struct YieldRangeInfo(
    [PbsType(PbsFieldType.UnsignedInteger)] int Min,
    [PbsType(PbsFieldType.PositiveInteger)] int Max
);

[PbsData("berry_plants")]
public class BerryPlantInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    [PbsType(PbsFieldType.PositiveInteger)]
    public int HoursPerStage { get; set; } = 3;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int DryingPerHour { get; set; } = 15;

    public YieldRangeInfo Yield { get; set; } = new(2, 5);
}
