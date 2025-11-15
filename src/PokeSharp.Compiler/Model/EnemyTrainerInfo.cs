using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public readonly record struct TrainerKey(
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(TrainerType))] Name TrainerType,
    Text Name,
    int Version = 0
)
{
    public override string ToString() => Version > 0 ? $"{TrainerType},{Name},{Version}" : $"{TrainerType},{Name}";
}

[PbsData("trainers")]
public class EnemyTrainerInfo
{
    [PbsSectionName]
    public required TrainerKey Id { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public ImmutableArray<Name> Items { get; set; } = [];

    [PbsType(PbsFieldType.UnformattedText)]
    public Text LoseText { get; set; }

    [PbsIgnore]
    public List<TrainerPokemonInfo> Pokemon { get; init; } = [];
}

public class TrainerPokemonInfo
{
    [PbsIgnore]
    public required Name Species { get; init; }

    [PbsIgnore]
    public required int Level { get; init; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int? Form { get; set; }

    public Text? Name { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))]
    public List<Name>? Moves { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Ability))]
    public Name? Ability { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int? AbilityIndex { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public Name? Item { get; set; }

    public PokemonGender? Gender { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Nature))]
    public Name? Nature { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger, FixedSize = 6, FixedSizeIsMax = true)]
    public List<int>? IV { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger, FixedSize = 6, FixedSizeIsMax = true)]
    public List<int>? EV { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int? Happiness { get; set; }

    public bool? Shiny { get; set; }

    public bool? SuperShiny { get; set; }

    public bool? Shadow { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public Name? Ball { get; set; }
}
