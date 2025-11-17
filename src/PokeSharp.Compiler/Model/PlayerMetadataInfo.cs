using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Core;

namespace PokeSharp.Compiler.Model;

[PbsData("metadata")]
public partial class PlayerMetadataInfo
{
    [PbsSectionName]
    [PbsType(PbsFieldType.UnsignedInteger)]
    public required int Id { get; init; }

    public required Name TrainerType { get; init; }

    public required string WalkCharset { get; init; }

    public string RunCharset { get; set; } = null!;

    public string CycleCharset { get; set; } = null!;

    public string SurfCharset { get; set; } = null!;

    public string DiveCharset { get; set; } = null!;

    public string FishCharset { get; set; } = null!;

    public string SurfFishCharset { get; set; } = null!;

    public HomeLocationInfo? Home { get; set; }
}
