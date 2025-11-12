using System.Collections.Immutable;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MessagePack;
using PokeSharp.Abstractions;
using PokeSharp.Data.Core;
using PokeSharp.SourceGenerator.Attributes;

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
public record EvolutionInfo(Name Species, Name EvolutionMethod, object? Parameter = null, bool IsPrevious = false);

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

[GameDataEntity(DataPath = "pokemon")]
[MessagePackObject(true)]
public partial record Species
{
    public static Species GetSpeciesForm(Name species, int form)
    {
        return Get(new SpeciesForm(species, form));
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

    public int UnmegaForm { get; init; } = 0;

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

    public IEnumerable<EvolutionInfo> GetEvolutions(bool excludeInvalid = false)
    {
        return Evolutions.Where(evo => !evo.IsPrevious && (!evo.EvolutionMethod.IsNone || !excludeInvalid));
    }

    public IEnumerable<EvolutionFamily> GetFamilyEvolutions(bool excludeInvalid = true)
    {
        return GetEvolutions(excludeInvalid)
            .OrderBy(e => Keys.Index().Where(i => i.Item == e.Species).Select(i => i.Index).FirstOrDefault())
            .Select(e => new EvolutionFamily(SpeciesId, e.Species, e.EvolutionMethod, e.Parameter, e.IsPrevious))
            .SelectMany(e => new[] { e }.Concat(Get(e.Species).GetFamilyEvolutions(excludeInvalid)));
    }

    [IgnoreMember]
    [JsonIgnore]
    public Name PreviousSpecies
    {
        get
        {
            foreach (var evo in Evolutions.Where(evo => evo.IsPrevious))
            {
                return evo.Species;
            }

            return SpeciesId;
        }
    }

    public Name GetBabySpecies(bool checkItems = false, Name item1 = default, Name item2 = default)
    {
        if (Evolutions.Length == 0)
            return SpeciesId;

        var result = SpeciesId;
        foreach (var evo in Evolutions.Where(evo => !evo.IsPrevious))
        {
            if (checkItems)
            {
                var incense = Get(PreviousSpecies).Incense;
                if (incense.IsNone || incense == item1 || incense == item2)
                {
                    result = PreviousSpecies;
                }
            }
            else
            {
                result = PreviousSpecies;
            }
        }

        return result != SpeciesId ? Get(result).GetBabySpecies(checkItems, item1, item2) : result;
    }

    public IEnumerable<Name> GetFamilySpecies()
    {
        var babySpecies = GetBabySpecies();
        yield return babySpecies;

        foreach (var evo in GetFamilyEvolutions(false))
        {
            yield return evo.Species;
        }
    }

    public bool BreedingCanProduce(Name otherSpecies)
    {
        var otherFamily = Get(otherSpecies).GetFamilySpecies();
        return Offspring.Length > 0 ? otherFamily.Intersect(Offspring).Any() : otherFamily.Contains(SpeciesId);
    }

    public ImmutableArray<Name> GetEggMoves()
    {
        if (EggMoves.Length > 0)
            return EggMoves;

        var previousSpecies = PreviousSpecies;
        return previousSpecies != SpeciesId ? GetSpeciesForm(previousSpecies, Form).GetEggMoves() : EggMoves;
    }

    public bool FamilyEvolutionsHaveMethod(Name method, object? parameter = null)
    {
        foreach (var evo in Get(GetBabySpecies()).GetFamilyEvolutions())
        {
            if (evo.EvolutionMethod != method)
                continue;

            if (parameter is not null && parameter.Equals(evo.Parameter))
                return true;
        }

        return false;
    }

    public bool FamilyItemEvolutionsUseItem(Name item = default)
    {
        foreach (var evo in Get(GetBabySpecies()).GetFamilyEvolutions())
        {
            if (Evolution.Get(evo.EvolutionMethod).UseItemProc is null)
                continue;

            if (item.IsValid && item.Equals(evo.Parameter))
                return true;
        }

        return false;
    }

    [IgnoreMember]
    [JsonIgnore]
    public int MinimumLevel
    {
        get
        {
            if (Evolutions.Length == 0)
                return 1;

            var evo = Evolutions.FirstOrDefault(e => !e.IsPrevious);
            if (evo is null)
                return 1;

            var previousData = GetSpeciesForm(evo.Species, BaseForm);
            var previousMinLevel = previousData.MinimumLevel;

            if (previousData.Incense.IsValid)
                return 1;

            var evolutionMethodData = Evolution.Get(evo.EvolutionMethod);
            if (evolutionMethodData.LevelUpProc is null && evolutionMethodData.Id != Evolution.Shedinja)
                return previousMinLevel;

            return evolutionMethodData.AnyLevelUp ? previousMinLevel + 1 : (int)evo.Parameter!;
        }
    }

    [GeneratedRegex("DefaultForm_(\\d+)")]
    private static partial Regex DefaultFormPattern { get; }
}
