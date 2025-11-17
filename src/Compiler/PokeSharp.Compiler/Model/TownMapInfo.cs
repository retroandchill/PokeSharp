using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Core;
using PokeSharp.Data;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public readonly record struct MapPointInfo(
    [PbsType(PbsFieldType.UnsignedInteger)] int X,
    [PbsType(PbsFieldType.UnsignedInteger)] int Y,
    Text Name,
    Text? PointOfInterest = null,
    [PbsType(PbsFieldType.UnsignedInteger)] int? FlyDestinationMap = null,
    [PbsType(PbsFieldType.UnsignedInteger)] int? FlyDestinationX = null,
    [PbsType(PbsFieldType.UnsignedInteger)] int? FlyDestinationY = null,
    [PbsType(PbsFieldType.UnsignedInteger)] int? Switch = null
);

[PbsData("town_map")]
public partial class TownMapInfo
{
    [PbsSectionName]
    [PbsType(PbsFieldType.UnsignedInteger)]
    public required int Id { get; init; }

    public Text Name { get; set; } = TextConstants.ThreeQuestions;

    public required string Filename { get; set; }

    [PbsKeyRepeat]
    [PbsKeyName("Point")]
    public List<MapPointInfo> Points { get; set; } = [];

    public List<string> Flags { get; set; } = [];
}
