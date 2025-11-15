using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public readonly record struct HomeLocationInfo(
    [PbsType(PbsFieldType.PositiveInteger)] int MapId,
    [PbsType(PbsFieldType.UnsignedInteger)] int X,
    [PbsType(PbsFieldType.UnsignedInteger)] int Y,
    [PbsType(PbsFieldType.UnsignedInteger)] int Direction
);

[PbsData("metadata")]
public class MetadataInfo
{
    [PbsSectionName]
    [PbsType(PbsFieldType.UnsignedInteger)]
    public required int Id { get; init; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int StartMoney { get; init; } = 3000;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public List<Name> StartItemStorage { get; init; } = [];

    public HomeLocationInfo? Home { get; init; }

    public Text? StorageCreator { get; init; }

    public string? WildBattleBGM { get; init; }

    public string? TrainerBattleBGM { get; init; }

    public string? WildVictoryBGM { get; init; }

    public string? TrainerVictoryBGM { get; init; }

    public string? WildCaptureME { get; init; }

    public string? SurfBGM { get; init; }

    public string? BicycleBGM { get; init; }
}
