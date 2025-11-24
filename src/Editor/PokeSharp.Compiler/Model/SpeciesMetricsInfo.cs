using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public readonly record struct SpeciesMetricsIdentifierInfo(
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Species))] Name Species,
    [PbsType(PbsFieldType.PositiveInteger)] int Form = 0
)
{
    public override string ToString() => Form > 0 ? $"{Species},{Form}" : Species.ToString();
}

[PbsData("pokemon_metrics")]
public partial class SpeciesMetricsInfo
{
    [PbsSectionName]
    public required SpeciesMetricsIdentifierInfo Id { get; init; }

    public Point BackSprite { get; set; }

    public Point FrontSprite { get; set; }

    [PbsWriteValidation(nameof(ValidateFrontSpriteAltitude))]
    public int FrontSpriteAltitude { get; set; }

    public int ShadowX { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int ShadowSize { get; set; }

    private static bool ValidateFrontSpriteAltitude(int value) => value != 0;
}
