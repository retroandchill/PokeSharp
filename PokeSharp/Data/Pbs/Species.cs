using System.Collections.Immutable;
using System.Text.RegularExpressions;
using PokeSharp.Abstractions;
using PokeSharp.Data.HardCoded;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

public readonly record struct SpeciesForm(Name Species, int Form = 0)
{
    public static implicit operator SpeciesForm(Name species) => new(species);
}

public readonly record struct LevelUpMove(Name Move, int Level);

public readonly record struct EvolutionInfo(Name Species, Name EvolutionMethod, object? Parameter = null);

public readonly record struct EvolutionFamily(
    Name PreviousSpecies,
    Name Species,
    Name EvolutionMethod,
    object? Parameter = null);

public enum MegaMessageType
{
    Normal,
    Move
}

[GameDataEntity(DataPath = "pokemon")]
public partial record Species
{
    public static Species GetSpeciesForm(Name species, int form)
    {
        return Get(new SpeciesForm(species, form));
    }

    public static IEnumerable<Species> AllSpecies => Entities.Where(e => e.Form == 0);
    
    public static int SpeciesCount => AllSpecies.Count();
    
    public required SpeciesForm Id { get; init; }

    public Name SpeciesId => Id.Species;
    public int Form => Id.Form;

    public Text Name { get; init; } = TextConstants.Unnamed;

    public Text? FormName { get; init; }

    public Text Category { get; init; } = TextConstants.ThreeQuestions;

    public Text PokedexEntry { get; init; } = TextConstants.ThreeQuestions;

    private readonly int? _pokedexForm;

    public int PokedexForm
    {
        get => _pokedexForm ?? Form;
        init => _pokedexForm = value;
    }

    public ImmutableArray<Name> Types { get; init; } = ["NORMAL"];

    public IReadOnlyDictionary<Name, int> BaseStats { get; init; } = ImmutableDictionary<Name, int>.Empty;

    public IReadOnlyDictionary<Name, int> EVs { get; init; } = ImmutableDictionary<Name, int>.Empty;

    public int BaseExp { get; init; } = 100;

    public Name GrowthRate { get; init; } = HardCoded.GrowthRate.Medium;

    public Name GenderRatio { get; init; } = HardCoded.GenderRatio.Female50Percent;

    public int CatchRate { get; init; } = 255;

    public int Happiness { get; init; } = 70;

    public ImmutableArray<LevelUpMove> LevelUpMoves { get; init; } = [];

    public ImmutableArray<Name> TutorMoves { get; init; } = [];

    public ImmutableArray<Name> EggMoves { get; init; } = [];

    public ImmutableArray<Name> Abilities { get; init; } = [];

    public ImmutableArray<Name> HiddenAbilities { get; init; } = [];

    public ImmutableArray<Name> WildItemCommon { get; init; } = [];

    public ImmutableArray<Name> WildItemUncommon { get; init; } = [];

    public ImmutableArray<Name> WildItemRare { get; init; } = [];

    public ImmutableArray<Name> EggGroups { get; init; } = [EggGroup.Undiscovered];

    public int HatchSteps { get; init; } = 1;
    
    public Name Incense { get; init; }
    
    public ImmutableArray<Name> Offspring { get; init; } = [];
    
    public ImmutableArray<EvolutionInfo> Evolutions { get; init; } = [];
    
    public EvolutionInfo? PreviousEvolution { get; init; }

    public Name PreviousSpecies => PreviousEvolution?.Species ?? SpeciesId;

    public float Height { get; init; } = 1;
    
    public float Weight { get; init; } = 1;

    public Name Color { get; init; } = BodyColor.Red;

    public Name Shape { get; init; } = BodyShape.Head;
    
    public Name Habitat { get; init; }

    public int Generation { get; init; } = 0;
    
    public IReadOnlySet<Name> Flags { get; init; } = ImmutableHashSet<Name>.Empty;
    
    public Name MegaStone { get; init; }
    
    public Name MegaMove { get; init; }
    
    public int UnmegaForm { get; init; } = 0;
    
    public MegaMessageType MegaMessage { get; init; }
    
    public int? DefaultForm
    {
        get
        {
            var regex = new Regex("DefaultForm_(\\d+)");
            foreach (var flag in Flags) {
                var match = regex.Match(flag);
                if (match.Success)
                {
                    return int.Parse(match.Groups[1].Value);
                }
            }

            return null;
        }
    }
    
    public int BaseForm => DefaultForm ?? Form;
    
    public bool IsSingleGendered => HardCoded.GenderRatio.Get(GenderRatio).IsSingleGender;
    
    public int BaseStatTotal => BaseStats.Values.Sum();
    
    public bool HasFlag(Name flag) => Flags.Contains(flag);

    public IEnumerable<EvolutionFamily> FamilyEvolutions
    {
        get
        {
            return Evolutions
                .OrderBy(e => Keys.Index()
                    .Where(x => x.Item == e.Species)
                    .Select(x => x.Item)
                    .FirstOrDefault())
                .SelectMany(x =>
                {
                    var firstSpecies = new EvolutionFamily[] { new(SpeciesId, x.Species, x.EvolutionMethod, x.Parameter) };
                    return firstSpecies.Concat(Species.Get(x.Species).FamilyEvolutions);
                });
        }
    }

    public Name GetBabySpecies(bool checkItems = false, Name item1 = default, Name item2 = default)
    {
        if (!PreviousEvolution.HasValue) return SpeciesId;
        
        var current = SpeciesId;
        if (checkItems)
        {
            var incense = Get(PreviousSpecies).Incense;
            if (incense.IsNone || incense == item1 || incense == item2)
            {
                current = PreviousSpecies;
            }
        }
        else
        {
            current = PreviousSpecies;
        }

        return current != SpeciesId ? Get(current).GetBabySpecies(checkItems, item1, item2) : current;
    }

    public IEnumerable<Name> FamilySpecies
    {
        get
        {
            var babySpecies = GetBabySpecies();
            yield return babySpecies;

            foreach (var evo in FamilyEvolutions)
            {
                yield return evo.Species;
            }
        }
    }

    public bool BreedingCanProduce(Name otherSpecies)
    {
        var otherFamily = Get(otherSpecies).FamilySpecies;
        return Offspring.Length > 0 ? otherFamily.Intersect(Offspring).Any() : otherFamily.Contains(SpeciesId);
    }

    public ImmutableArray<Name> EffectiveEggMoves
    {
        get
        {
            if (EggMoves.Length > 0) return EggMoves;

            return PreviousEvolution.HasValue ? GetSpeciesForm(PreviousSpecies, Form).EffectiveEggMoves : [];
        }
    }

    public bool FamilyEvolutionsHaveMethod(Name method, object? parameter = null)
    {
        foreach (var evo in Get(GetBabySpecies()).FamilyEvolutions)
        {
            if (evo.EvolutionMethod != method) continue;
            
            if (parameter is not null && parameter.Equals(evo.Parameter)) return true;
        }
        
        return false;
    }

    public bool FamilyItemEvolutionsUseItem(Name item = default)
    {
        foreach (var evo in Get(GetBabySpecies()).FamilyEvolutions)
        {
            if (Evolution.Get(evo.EvolutionMethod).UseItemProc is null) continue;
            
            if (item.IsValid && item.Equals(evo.Parameter)) return true;
        }

        return false;
    }

    public int MinimumLevel
    {
        get
        {
            if (!PreviousEvolution.HasValue) return 1;

            var previousData = GetSpeciesForm(PreviousSpecies, BaseForm);
            if (previousData.Incense.IsValid) return 1;
            
            var previousMinimumLevel = previousData.MinimumLevel;
            var evoMethodData = Evolution.Get(PreviousEvolution.Value.EvolutionMethod);
            if (evoMethodData.LevelUpProc is null && evoMethodData.Id != Evolution.Shedinja)
            {
                return previousMinimumLevel;
            }
            
            return evoMethodData.AnyLevelUp ? previousMinimumLevel + 1 : (int) PreviousEvolution.Value.Parameter!;
        }
    }
}