using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

public enum TrainerGender : byte
{
    Male,
    Female,
    Unknown,
    Mixed,
}

[GameDataEntity(DataPath = "trainer_types")]
public partial record TrainerType
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    public required TrainerGender Gender { get; init; }

    public bool IsMale => Gender == TrainerGender.Male;

    public bool IsFemale => Gender == TrainerGender.Female;

    public required int BaseMoney { get; init; }

    public required int SkillLevel { get; init; }

    public required ImmutableArray<Name> Flags { get; init; }

    public required string? IntroBGM { get; init; }

    public required string? BattleBGM { get; init; }

    public required string? VictoryBGM { get; init; }

    public bool HasFlag(Name flag) => Flags.Contains(flag);
}
