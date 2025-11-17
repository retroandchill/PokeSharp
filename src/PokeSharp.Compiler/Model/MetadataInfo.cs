using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public readonly record struct HomeLocationInfo(
    [PbsType(PbsFieldType.PositiveInteger)] int MapId,
    [PbsType(PbsFieldType.UnsignedInteger)] int X,
    [PbsType(PbsFieldType.UnsignedInteger)] int Y,
    [PbsType(PbsFieldType.UnsignedInteger)] int Direction
);

[PbsData("metadata")]
public partial class MetadataInfo
{
    [PbsSectionName]
    [PbsType(PbsFieldType.UnsignedInteger)]
    public required int Id { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int StartMoney { get; set; } = 3000;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public List<Name> StartItemStorage { get; set; } = [];

    public required HomeLocationInfo Home { get; init; }

    public Text StorageCreator { get; set; } = Text.Localized("Metadata.StorageCreator", "Bill", "Bill");

    public string? WildBattleBGM { get; set; }

    public string? TrainerBattleBGM { get; set; }

    public string? WildVictoryBGM { get; set; }

    public string? TrainerVictoryBGM { get; set; }

    public string? WildCaptureME { get; set; }

    public string? SurfBGM { get; set; }

    public string? BicycleBGM { get; set; }
}
