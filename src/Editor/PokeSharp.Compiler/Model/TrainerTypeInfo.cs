using System.Collections.Immutable;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Core;
using PokeSharp.Data;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

[PbsData("trainer_types")]
public partial class TrainerTypeInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; set; } = TextConstants.Unnamed;

    public TrainerGender Gender { get; set; } = TrainerGender.Unknown;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int BaseMoney { get; set; } = 30;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int? SkillLevel { get; set; }

    public List<string> Flags { get; set; } = [];

    public string? IntroBGM { get; set; }

    public string? BattleBGM { get; set; }

    public string? VictoryBGM { get; set; }
}
