using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public readonly record struct EVYieldInfo(
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Stat))] Name Stat,
    [PbsType(PbsFieldType.PositiveInteger)] int Amount
);

public readonly record struct LevelUpMoveInfo(
    [PbsType(PbsFieldType.UnsignedInteger)] int Level,
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))] Name Move
);

public readonly record struct EvolutionMethodInfo(
    Name Species,
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Evolution), AllowNone = true)] Name Method,
    string? Parameter = null
);

[PbsData("pokemon")]
public class SpeciesInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; set; } = TextConstants.Unnamed;

    public Text? FormName { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(PokemonType))]
    public List<Name> Types { get; set; } = ["NORMAL"];

    [PbsType(PbsFieldType.PositiveInteger, FixedSize = 6)]
    public List<int> BaseStats { get; set; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(GenderRatio))]
    public Name GenderRatio { get; set; } = PokeSharp.Data.Core.GenderRatio.Female50Percent.Id;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(GrowthRate))]
    public Name GrowthRate { get; set; } = PokeSharp.Data.Core.GrowthRate.Medium.Id;

    [PbsType(PbsFieldType.PositiveInteger)]
    public int BaseExp { get; set; } = 100;

    public List<EVYieldInfo> EVs { get; set; } = [];

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int CatchRate { get; set; } = 255;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int Happiness { get; set; } = 70;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Ability))]
    public List<Name> Abilities { get; set; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Ability))]
    public List<Name> HiddenAbilities { get; set; } = [];

    public List<LevelUpMoveInfo> Moves { get; set; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))]
    public List<Name> TutorMoves { get; set; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))]
    public List<Name> EggMoves { get; set; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(EggGroup))]
    public List<Name> EggGroups { get; set; } = [EggGroup.Undiscovered.Id];

    [PbsType(PbsFieldType.PositiveInteger)]
    public int HatchSteps { get; set; } = 1;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public Name Incense { get; set; }

    public List<Name> Offspring { get; set; } = [];

    public decimal Height { get; set; } = 1;

    public decimal Weight { get; set; } = 1;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(BodyColor))]
    public Name Color { get; set; } = BodyColor.Red.Id;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(BodyShape))]
    public Name Shape { get; set; } = BodyShape.Head.Id;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Habitat))]
    public Name Habitat { get; set; }

    public Text Category { get; set; } = TextConstants.ThreeQuestions;

    [PbsType(PbsFieldType.UnformattedText)]
    public Text Pokedex { get; set; } = TextConstants.ThreeQuestions;

    public int Generation { get; set; }

    public List<string> Flags { get; set; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public List<Name> WildItemCommon { get; set; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public List<Name> WildItemUncommon { get; set; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public List<Name> WildItemRare { get; set; } = [];

    [PbsKeyName("Evolution")]
    [PbsKeyRepeat]
    public List<EvolutionMethodInfo> Evolutions { get; set; } = [];
}

public readonly record struct SpeciesFormIdentifierInfo(
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Species))] Name Species,
    [PbsType(PbsFieldType.PositiveInteger)] int Form
)
{
    public override string ToString() => $"{Species},{Form}";
}

public readonly record struct FormEvolutionMethodInfo(
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Species))] Name Species,
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Evolution), AllowNone = true)] Name Method,
    string? Parameter = null
);

[PbsData("pokemon_forms")]
public class SpeciesFormInfo
{
    [PbsSectionName]
    public required SpeciesFormIdentifierInfo Id { get; init; }

    public Text? FormName { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int PokedexForm { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public Name MegaStone { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))]
    public Name MegaMove { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int UnmegaForm { get; set; }

    [PbsType(PbsFieldType.EnumerableOrInteger)]
    public MegaMessageType MegaMessage { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(PokemonType))]
    public List<Name>? Types { get; set; }

    [PbsType(PbsFieldType.PositiveInteger, FixedSize = 6)]
    public List<int>? BaseStats { get; set; }

    [PbsType(PbsFieldType.PositiveInteger)]
    public int BaseExp { get; set; }

    public List<EVYieldInfo>? EVs { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int CatchRate { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int Happiness { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Ability))]
    public List<Name>? Abilities { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Ability))]
    public List<Name>? HiddenAbilities { get; set; }

    public List<LevelUpMoveInfo>? Moves { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))]
    public List<Name>? TutorMoves { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))]
    public List<Name>? EggMoves { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(EggGroup))]
    public List<Name>? EggGroups { get; set; }

    [PbsType(PbsFieldType.PositiveInteger)]
    public int HatchSteps { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Species))]
    public List<Name>? Offspring { get; set; }

    public decimal Height { get; set; }

    public decimal Weight { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(BodyColor))]
    public Name Color { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(BodyShape))]
    public Name Shape { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Habitat))]
    public Name Habitat { get; set; }

    public Text Category { get; set; }

    [PbsType(PbsFieldType.UnformattedText)]
    public Text Pokedex { get; set; }

    public int Generation { get; set; }

    public List<string> Flags { get; set; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public List<Name>? WildItemCommon { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public List<Name>? WildItemUncommon { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public List<Name>? WildItemRare { get; set; }

    [PbsKeyName("Evolution")]
    [PbsKeyRepeat]
    public List<FormEvolutionMethodInfo>? Evolutions { get; set; }
}
