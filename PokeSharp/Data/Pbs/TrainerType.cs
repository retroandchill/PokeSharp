using System.Collections.Immutable;
using System.Text.Json.Serialization;
using MessagePack;
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
[MessagePackObject(true)]
public partial record TrainerType
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    public required TrainerGender Gender { get; init; }

    [IgnoreMember]
    [JsonIgnore]
    public bool IsMale => Gender == TrainerGender.Male;

    [IgnoreMember]
    [JsonIgnore]
    public bool IsFemale => Gender == TrainerGender.Female;

    public required int BaseMoney { get; init; }

    public required int SkillLevel { get; init; }

    public required ImmutableArray<Name> Flags { get; init; }

    public required string? IntroBGM { get; init; }

    public required string? BattleBGM { get; init; }

    public required string? VictoryBGM { get; init; }

    /// <summary>
    /// Determines whether the specified flag is present in the list of flags.
    /// </summary>
    /// <param name="flag">The flag to check for in the list of flags.</param>
    /// <returns>
    /// <c>true</c> if the specified flag is present in the list of flags; otherwise, <c>false</c>.
    /// </returns>
    public bool HasFlag(Name flag) => Flags.Contains(flag);
}
