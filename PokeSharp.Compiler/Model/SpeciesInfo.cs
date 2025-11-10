using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public readonly record struct EVYieldInfo(
    [property: PbsType(PbsFieldType.Enumerable, EnumType = typeof(Stat))] Name Stat,
    [property: PbsType(PbsFieldType.PositiveInteger)] int Amount
);

public readonly record struct LevelUpMoveInfo(
    [property: PbsType(PbsFieldType.UnsignedInteger)] int Level,
    [property: PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))] Name Move
);

public readonly record struct EvolutionMethodInfo(
    Name Species,
    [property: PbsType(PbsFieldType.Enumerable, EnumType = typeof(Evolution), AllowNone = true)]
        Name Method,
    string? Parameter = null
);

[PbsData("pokemon")]
public record SpeciesInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; init; } = TextConstants.Unnamed;

    public Text? FormName { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(PokemonType))]
    public ImmutableArray<Name> Types { get; init; } = ["NORMAL"];

    [PbsType(PbsFieldType.PositiveInteger, FixedSize = 6)]
    public ImmutableArray<int> BaseStats { get; init; } = [];

    [property: PbsType(PbsFieldType.Enumerable, EnumType = typeof(GenderRatio))]
    public Name GenderRatio { get; init; } = PokeSharp.Data.Core.GenderRatio.Female50Percent;

    [property: PbsType(PbsFieldType.Enumerable, EnumType = typeof(GrowthRate))]
    public Name GrowthRate { get; init; } = PokeSharp.Data.Core.GrowthRate.Medium;

    [PbsType(PbsFieldType.PositiveInteger)]
    public int BaseExp { get; init; } = 100;

    public ImmutableArray<EVYieldInfo> EVs { get; init; } = [];

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int CatchRate { get; init; } = 255;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int Happiness { get; init; } = 70;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Ability))]
    public ImmutableArray<Name> Abilities { get; init; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Ability))]
    public ImmutableArray<Name> HiddenAbilities { get; init; } = [];

    public ImmutableArray<LevelUpMoveInfo> Moves { get; init; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))]
    public ImmutableArray<Name> TutorMoves { get; init; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))]
    public ImmutableArray<Name> EggMoves { get; init; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(EggGroup))]
    public ImmutableArray<Name> EggGroups { get; init; } = [EggGroup.Undiscovered];

    [PbsType(PbsFieldType.PositiveInteger)]
    public int HatchSteps { get; init; } = 1;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public Name Incense { get; init; }

    public ImmutableArray<Name> Offspring { get; init; } = [];

    public decimal Height { get; init; } = 1;

    public decimal Weight { get; init; } = 1;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(BodyColor))]
    public Name Color { get; init; } = BodyColor.Red;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(BodyShape))]
    public Name Shape { get; init; } = BodyShape.Head;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Habitat))]
    public Name Habitat { get; init; }

    public Text Category { get; init; } = TextConstants.ThreeQuestions;

    [PbsType(PbsFieldType.UnformattedText)]
    public Text Pokedex { get; init; } = TextConstants.ThreeQuestions;

    public int Generation { get; init; } = 0;

    public ImmutableArray<string> Flags { get; init; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public ImmutableArray<Name> WildItemCommon { get; init; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public ImmutableArray<Name> WildItemUncommon { get; init; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public ImmutableArray<Name> WildItemRare { get; init; } = [];

    [PbsKeyName("Evolution")]
    [PbsKeyRepeat]
    public ImmutableArray<EvolutionMethodInfo> Evolutions { get; init; } = [];
}

public readonly record struct SpeciesFormIdentifierInfo(
    [property: PbsType(PbsFieldType.Enumerable, EnumType = typeof(Species))] Name Species,
    [property: PbsType(PbsFieldType.PositiveInteger)] int Form
)
{
    public override string ToString() => $"{Species},{Form}";
}

public readonly record struct FormEvolutionMethodInfo(
    [property: PbsType(PbsFieldType.Enumerable, EnumType = typeof(Species))] Name Species,
    [property: PbsType(PbsFieldType.Enumerable, EnumType = typeof(Evolution), AllowNone = true)]
        Name Method,
    string? Parameter = null
);

[PbsData("pokemon_forms")]
public record SpeciesFormInfo
{
    [PbsSectionName]
    public required SpeciesFormIdentifierInfo Id { get; init; }

    public Text? FormName { get; init; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int PokedexForm { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public Name MegaStone { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))]
    public Name MegaMove { get; init; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int UnmegaForm { get; init; }

    [PbsType(PbsFieldType.EnumerableOrInteger)]
    public MegaMessageType MegaMessage { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(PokemonType))]
    public ImmutableArray<Name> Types { get; init; }

    [PbsType(PbsFieldType.PositiveInteger, FixedSize = 6)]
    public ImmutableArray<int> BaseStats { get; init; }

    [PbsType(PbsFieldType.PositiveInteger)]
    public int BaseExp { get; init; }

    public ImmutableArray<EVYieldInfo> EVs { get; init; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int CatchRate { get; init; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int Happiness { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Ability))]
    public ImmutableArray<Name> Abilities { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Ability))]
    public ImmutableArray<Name> HiddenAbilities { get; init; }

    public ImmutableArray<LevelUpMoveInfo> Moves { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))]
    public ImmutableArray<Name> TutorMoves { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move))]
    public ImmutableArray<Name> EggMoves { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(EggGroup))]
    public ImmutableArray<Name> EggGroups { get; init; }

    [PbsType(PbsFieldType.PositiveInteger)]
    public int HatchSteps { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Species))]
    public ImmutableArray<Name> Offspring { get; init; }

    public decimal Height { get; init; }

    public decimal Weight { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(BodyColor))]
    public Name Color { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(BodyShape))]
    public Name Shape { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Habitat))]
    public Name Habitat { get; init; }

    public Text Category { get; init; }

    [PbsType(PbsFieldType.UnformattedText)]
    public Text Pokedex { get; init; }

    public int Generation { get; init; }

    public ImmutableArray<string> Flags { get; init; } = [];

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public ImmutableArray<Name> WildItemCommon { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public ImmutableArray<Name> WildItemUncommon { get; init; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Item))]
    public ImmutableArray<Name> WildItemRare { get; init; }

    [PbsKeyName("Evolution")]
    [PbsKeyRepeat]
    public ImmutableArray<FormEvolutionMethodInfo> Evolutions { get; init; }
}
