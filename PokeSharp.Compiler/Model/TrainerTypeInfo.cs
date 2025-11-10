using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

[PbsData("trainer_types")]
public record TrainerTypeInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; init; } = TextConstants.Unnamed;

    public TrainerGender Gender { get; init; } = TrainerGender.Unknown;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int BaseMoney { get; init; } = 30;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int? SkillLevel { get; init; }

    public ImmutableArray<string> Flags { get; init; } = [];

    public string? IntroBGM { get; init; }

    public string? BattleBGM { get; init; }

    public string? VictoryBGM { get; init; }
}
