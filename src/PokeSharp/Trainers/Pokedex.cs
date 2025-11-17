using PokeSharp.Core;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using PokeSharp.PokemonModel;
using PokeSharp.Settings;
using PokeSharp.Utilities;

namespace PokeSharp.Trainers;

public enum PokedexGender
{
    MaleOrOneForm,
    Female,
    Male = MaleOrOneForm,
    OneForm = MaleOrOneForm,
}

public static class PokedexGenderExtensions
{
    public static PokedexGender ToPokedexGender(this PokemonGender gender)
    {
        return gender switch
        {
            PokemonGender.Male => PokedexGender.Male,
            PokemonGender.Female => PokedexGender.Female,
            PokemonGender.Genderless => PokedexGender.OneForm,
            _ => throw new ArgumentOutOfRangeException(nameof(gender), gender, null),
        };
    }
}

public readonly record struct SeenForm(PokedexGender Gender, bool Shiny, int Form);

public class Pokedex
{
    public const int NationalDex = -1;

    public List<int> AccessibleDexes { get; } = [];
    private readonly bool[] _unlockedDexes;
    private readonly HashSet<Name> _seen = [];
    private readonly HashSet<Name> _owned = [];
    private readonly Dictionary<Name, HashSet<SeenForm>> _seenForms = new();
    private readonly HashSet<Name> _seenEggs = [];
    private readonly Dictionary<Name, SeenForm> _lastSeenForms = new();
    private readonly HashSet<Name> _ownedShadow = [];
    private readonly Dictionary<Name, int> _caughtCounts = new();
    private readonly Dictionary<Name, int> _defeatedCounts = new();

    public Pokedex()
    {
        _unlockedDexes = Enumerable.Range(0, RegionalDex.Count + 1).Select(i => i == 0).ToArray();

        RefreshAccessibleDexes();
    }

    public void Clear()
    {
        _seen.Clear();
        _owned.Clear();
        _seenForms.Clear();
        _seenEggs.Clear();
        _lastSeenForms.Clear();
        _ownedShadow.Clear();
        _caughtCounts.Clear();
        _defeatedCounts.Clear();
        RefreshAccessibleDexes();
    }

    public void SetSeen(Name species, bool shouldRefreshDexes = true)
    {
        _seen.Add(species);
        if (shouldRefreshDexes)
            RefreshAccessibleDexes();
    }

    public bool HasSeen(Name species) => _seen.Contains(species);

    public bool HasSeenForm(Name species, PokedexGender gender, int form, bool? shiny = null)
    {
        if (!_seenForms.TryGetValue(species, out var seenForms))
            return false;

        if (!shiny.HasValue)
        {
            return seenForms.Contains(new SeenForm(gender, false, form))
                || seenForms.Contains(new SeenForm(gender, true, form));
        }

        return seenForms.Contains(new SeenForm(gender, shiny.Value, form));
    }

    public void SetSeenEgg(Name species)
    {
        _seenEggs.Add(species);
    }

    public bool SeenEgg(Name species) => _seenEggs.Contains(species);

    public int SeenCount => CountSpecies(_seen);

    public int GetSeenCount(int dex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(dex);
        return CountSpecies(_seen, dex);
    }

    public bool SeenAny => HasSeenAny(NationalDex);

    public bool HasSeenAny(int dex)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(dex, NationalDex);
        return dex == NationalDex
            ? Species.AllSpecies.Any(s => _seen.Contains(s.SpeciesId))
            : RegionalDex.Get(dex).Entries.Any(s => _seen.Contains(s));
    }

    public int GetSeenFormCount(Name species)
    {
        return _seenForms.TryGetValue(species, out var set) ? set.Count : 0;
    }

    public SeenForm GetLastFormSeen(Name species)
    {
        return _lastSeenForms.TryGetValue(species, out var form)
            ? form
            : new SeenForm(PokedexGender.MaleOrOneForm, false, 0);
    }

    public void SetLastFormSeen(
        Name species,
        PokedexGender gender = PokedexGender.MaleOrOneForm,
        int form = 0,
        bool shiny = false
    )
    {
        _lastSeenForms[species] = new SeenForm(gender, shiny, form);
    }

    public void SetOwned(Name species, bool shouldRefreshDexes = true)
    {
        _owned.Add(species);
        if (shouldRefreshDexes)
        {
            RefreshAccessibleDexes();
        }
    }

    public void SetShadowPokemonOwned(Name species)
    {
        _ownedShadow.Add(species);
        RefreshAccessibleDexes();
    }

    public bool Owns(Name species) => _owned.Contains(species);

    public bool OwnsShadowPokemon(Name species) => _ownedShadow.Contains(species);

    public int OwnedCount => CountSpecies(_owned);

    public int GetOwnedCount(int dex) => CountSpecies(_owned, dex);

    public void Register(Pokemon pokemon, bool shouldRefreshDexes = true)
    {
        Register(pokemon.SpeciesData, pokemon.Gender, pokemon.Shiny, shouldRefreshDexes);
    }

    public void Register(
        Name species,
        PokemonGender gender = PokemonGender.Male,
        int form = 0,
        bool shiny = false,
        bool shouldRefreshDexes = true
    )
    {
        Register(Species.Get(species, form), gender, shiny, shouldRefreshDexes);
    }

    private void Register(Species species, PokemonGender gender, bool shiny, bool shouldRefreshDexes)
    {
        var pokedexGender = gender.ToPokedexGender();
        if (species.Form != species.PokedexForm)
        {
            species = Species.Get(species.SpeciesId, species.PokedexForm);
        }

        var form = species.FormName.GetValueOrDefault().AsReadOnlySpan().IsEmpty ? species.Form : 0;
        _seen.Add(species.SpeciesId);
        if (!_seenForms.TryGetValue(species.SpeciesId, out var seenForms))
        {
            seenForms = [];
            _seenForms[species.SpeciesId] = seenForms;
        }

        var seenForm = new SeenForm(pokedexGender, shiny, form);
        seenForms.Add(seenForm);

        _lastSeenForms.TryAdd(species.SpeciesId, seenForm);

        if (shouldRefreshDexes)
        {
            RefreshAccessibleDexes();
        }
    }

    public void RegisterLastSeen(Pokemon pokemon)
    {
        var speciesData = pokemon.SpeciesData;
        var form = speciesData.FormName.GetValueOrDefault().AsReadOnlySpan().IsEmpty ? speciesData.PokedexForm : 0;
        var gender = pokemon.Gender.ToPokedexGender();
        _lastSeenForms[pokemon.Species] = new SeenForm(gender, pokemon.Shiny, form);
    }

    public int GetCaughtCount(Name species) => _caughtCounts.GetValueOrDefault(species);

    public int GetDefeatedCount(Name species) => _defeatedCounts.GetValueOrDefault(species);

    public int BattledCount(Name species) => GetCaughtCount(species) + GetDefeatedCount(species);

    public void RegisterCaught(Name species)
    {
        if (!_caughtCounts.TryAdd(species, 1))
        {
            _caughtCounts[species]++;
        }
    }

    public void RegisterDefeated(Name species)
    {
        if (!_defeatedCounts.TryAdd(species, 1))
        {
            _defeatedCounts[species]++;
        }
    }

    public void Unlock(int dex)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(dex, NationalDex);
        if (dex == NationalDex)
            dex = _unlockedDexes.Length - 1;
        _unlockedDexes[dex] = true;
        RefreshAccessibleDexes();
    }

    public void Lock(int dex)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(dex, NationalDex);
        if (dex == NationalDex)
            dex = _unlockedDexes.Length - 1;
        _unlockedDexes[dex] = false;
        RefreshAccessibleDexes();
    }

    public bool IsUnlocked(int dex)
    {
        if (dex == NationalDex)
            dex = _unlockedDexes.Length - 1;
        return _unlockedDexes[dex];
    }

    public int DexesCount => _unlockedDexes.Length;

    private void RefreshAccessibleDexes()
    {
        AccessibleDexes.Clear();
        if (GameServices.GameSettings.UseCurrentRegionDex)
        {
            var region = GameplayUtils.CurrentRegion ?? NationalDex;
            if (region >= DexesCount - 1)
                region = NationalDex;
            AccessibleDexes.Add(region);
            return;
        }

        if (DexesCount == 1)
        {
            if (IsUnlocked(0) && SeenAny)
            {
                AccessibleDexes.Add(NationalDex);
            }
        }
        else
        {
            for (var i = 0; i < DexesCount; i++)
            {
                var dexListToCheck = (i == DexesCount - 1) ? NationalDex : i;
                if (IsUnlocked(i) && HasSeenAny(dexListToCheck))
                {
                    AccessibleDexes.Add(dexListToCheck);
                }
            }
        }
    }

    public bool IsSpeciesUnlockedInDex(Name species)
    {
        if (_unlockedDexes[^1])
            return true;

        return Enumerable
            .Range(0, _unlockedDexes.Length - 1)
            .Any(i => IsUnlocked(i) && GameplayUtils.GetRegionalNumber(i, species) > 0);
    }

    private static int CountSpecies(HashSet<Name> species, int region = NationalDex)
    {
        return region == NationalDex
            ? Species.AllSpecies.Count(s => species.Contains(s.SpeciesId))
            : RegionalDex.GetAllRegionalSpecies(region).Count(species.Contains);
    }
}
