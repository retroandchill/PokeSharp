using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

public enum TrainerGender : byte
{
    Male,
    Female,
    Unknown,
    Mixed
}

[GameDataEntity(DataPath = "trainer_types")]
public partial record TrainerType
{
    public required Name Id { get; init; }
    
    public Text Name { get; init; } = TextConstants.Unnamed;
    
    public TrainerGender Gender { get; init; } = TrainerGender.Unknown;
    
    public bool IsMale => Gender == TrainerGender.Male;
    
    public bool IsFemale => Gender == TrainerGender.Female;

    public int BaseMoney { get; init; } = 30;

    private readonly int? _skillLevel;
    public int SkillLevel
    {
        get => _skillLevel ?? BaseMoney;
        init => _skillLevel = value;
    }
    
    public IReadOnlySet<Name> Flags { get; init; } = ImmutableHashSet<Name>.Empty;
    
    public bool HasFlag(Name flag) => Flags.Contains(flag);
}