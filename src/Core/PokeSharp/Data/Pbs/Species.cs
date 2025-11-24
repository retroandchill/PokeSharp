using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MessagePack;
using MessagePack.Formatters;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Core;
using PokeSharp.Serialization.MessagePack;

namespace PokeSharp.Data.Pbs;

[MessagePackObject(true)]
public readonly record struct SpeciesForm(Name Species, int Form = 0)
{
    public static implicit operator SpeciesForm(Name species) => new(species);

    public static implicit operator SpeciesForm(string species) => new(species);

    public override string ToString() => Form > 0 ? $"{Species},{Form}" : Species.ToString();
}

[MessagePackObject(true)]
public readonly record struct LevelUpMove(Name Move, int Level);

[MessagePackObject(true)]
public record EvolutionInfo(
    Name Species,
    Name EvolutionMethod,
    [property: MessagePackFormatter(typeof(EvolutionInfoParameterFormatter))] object? Parameter = null,
    bool IsPrevious = false
);

public readonly record struct EvolutionFamily(
    Name PreviousSpecies,
    Name Species,
    Name EvolutionMethod,
    object? Parameter = null,
    bool IsPrevious = false
);

public enum MegaMessageType
{
    Normal,
    Move,
}

[GameDataEntity(DataPath = "species")]
[MessagePackObject(true)]
public partial record Species
{
    public static Species Get(Name species, int form = 0)
    {
        return Get(new SpeciesForm(species, form));
    }

    public static bool TryGet(Name species, [NotNullWhen(true)] out Species? result)
    {
        return TryGet(new SpeciesForm(species), out result);
    }

    public static bool TryGet(Name species, int form, [NotNullWhen(true)] out Species? result)
    {
        return TryGet(new SpeciesForm(species, form), out result);
    }

    public static IEnumerable<Species> AllSpecies => Entities.Where(e => e.Form == 0);

    public static int SpeciesCount => AllSpecies.Count();

    public required SpeciesForm Id { get; init; }

    [IgnoreMember]
    public Name SpeciesId => Id.Species;

    [IgnoreMember]
    public int Form => Id.Form;

    public required Text Name { get; init; }

    public required Text? FormName { get; init; }

    public required Text Category { get; init; }

    public required Text PokedexEntry { get; init; }

    private readonly int? _pokedexForm;

    public int PokedexForm
    {
        get => _pokedexForm ?? Form;
        init => _pokedexForm = value;
    }

    public required ImmutableArray<Name> Types { get; init; }

    public required IReadOnlyDictionary<Name, int> BaseStats { get; init; }

    public required IReadOnlyDictionary<Name, int> EVs { get; init; }

    public required int BaseExp { get; init; }

    public required Name GrowthRate { get; init; }

    public required Name GenderRatio { get; init; }

    public required int CatchRate { get; init; }

    public required int Happiness { get; init; }

    public required ImmutableArray<LevelUpMove> LevelUpMoves { get; init; }

    public required ImmutableArray<Name> TutorMoves { get; init; }

    public required ImmutableArray<Name> EggMoves { get; init; }

    public required ImmutableArray<Name> Abilities { get; init; }

    public required ImmutableArray<Name> HiddenAbilities { get; init; }

    public required ImmutableArray<Name> WildItemCommon { get; init; }

    public required ImmutableArray<Name> WildItemUncommon { get; init; }

    public required ImmutableArray<Name> WildItemRare { get; init; }

    public required ImmutableArray<Name> EggGroups { get; init; }

    public required int HatchSteps { get; init; }

    public required Name Incense { get; init; }

    public required ImmutableArray<Name> Offspring { get; init; }

    public required ImmutableArray<EvolutionInfo> Evolutions { get; init; }

    public required int Height { get; init; }

    public required int Weight { get; init; }

    public required Name Color { get; init; }

    public required Name Shape { get; init; }

    public required Name Habitat { get; init; }

    public required int Generation { get; init; }

    public required ImmutableArray<Name> Flags { get; init; }

    public Name MegaStone { get; init; }

    public Name MegaMove { get; init; }

    public int UnmegaForm { get; init; }

    public MegaMessageType MegaMessage { get; init; }

    [IgnoreMember]
    [JsonIgnore]
    public int? DefaultForm
    {
        get
        {
            foreach (var match in Flags.Select(flag => DefaultFormPattern.Match(flag)).Where(match => match.Success))
            {
                return int.Parse(match.Groups[1].Value);
            }

            return null;
        }
    }

    [IgnoreMember]
    [JsonIgnore]
    public int BaseForm => DefaultForm ?? Form;

    [IgnoreMember]
    [JsonIgnore]
    public bool IsSingleGendered => Core.GenderRatio.Get(GenderRatio).IsSingleGender;

    [IgnoreMember]
    [JsonIgnore]
    public int BaseStatTotal => BaseStats.Values.Sum();

    /// <summary>
    /// Determines whether the specified flag is present in the list of flags.
    /// </summary>
    /// <param name="flag">The flag to check for in the list of flags.</param>
    /// <returns>
    /// <c>true</c> if the specified flag is present in the list of flags; otherwise, <c>false</c>.
    /// </returns>
    public bool HasFlag(Name flag) => Flags.Contains(flag);

    [GeneratedRegex("DefaultForm_(\\d+)")]
    private static partial Regex DefaultFormPattern { get; }
}
