using PokeSharp.Core;
using PokeSharp.Core.Utils;
using PokeSharp.Data.Core;
using PokeSharp.PokemonModel;
using PokeSharp.Settings;

namespace PokeSharp.BattleSystem.Challenge;

public class PokemonChallengeRuleset(int number = 0)
{
    private int _number = number;
    public int Number
    {
        get => _number;
        set => SetNumberRange(value, value);
    }
    private readonly List<IPokemonRestriction> _pokemonRules = [];
    private readonly List<ITeamRestriction> _teamRules = [];
    private readonly List<ITeamRestriction> _subsetRules = [];

    public PokemonChallengeRuleset Copy()
    {
        var result = new PokemonChallengeRuleset(Number);
        foreach (var rule in _pokemonRules)
        {
            result.AddPokemonRule(rule);
        }

        foreach (var rule in _teamRules)
        {
            result.AddTeamRule(rule);
        }

        foreach (var rule in _subsetRules)
        {
            result.AddSubsetRule(rule);
        }

        return result;
    }

    public int MinLength
    {
        get => field > 0 ? field : MaxLength;
        private set;
    }

    public int MaxLength => Number < 0 ? GameGlobal.GameSettings.MaxPartySize : Number;

    public int MinTeamLength => Math.Max(MinLength, 1);

    public int MaxTeamLength => Math.Min(MaxLength, GameGlobal.GameSettings.MaxPartySize);

    public int SuggestedNumber => MaxLength;

    public int SuggestedLevel
    {
        get
        {
            var minLevel = 1;
            var maxLevel = GrowthRate.MaxLevel;
            var num = SuggestedNumber;
            foreach (var rule in _pokemonRules)
            {
                switch (rule)
                {
                    case MinimumLevelRestriction minLevelRule:
                        minLevel = minLevelRule.Level;
                        break;
                    case MaximumLevelRestriction maximumLevelRule:
                        maxLevel = maximumLevelRule.Level;
                        break;
                }
            }

            var totalLevel = maxLevel * num;
            foreach (var rule in _subsetRules.OfType<TotalLevelRestriction>())
            {
                totalLevel = rule.Level;
            }

            return totalLevel >= maxLevel * num
                ? Math.Max(maxLevel, minLevel)
                : Math.Max(totalLevel / SuggestedNumber, minLevel);
        }
    }

    public void SetNumberRange(int minValue, int maxValue)
    {
        MinLength = Math.Max(1, minValue);
        _number = Math.Max(1, maxValue);
    }

    public void AddTeamRule(ITeamRestriction rule)
    {
        _teamRules.Add(rule);
    }

    public void AddSubsetRule(ITeamRestriction rule)
    {
        _subsetRules.Add(rule);
    }

    public void AddPokemonRule(IPokemonRestriction rule)
    {
        _pokemonRules.Add(rule);
    }

    public void ClearTeamRules()
    {
        _teamRules.Clear();
    }

    public void ClearSubsetRules()
    {
        _subsetRules.Clear();
    }

    public void ClearPokemonRules()
    {
        _pokemonRules.Clear();
    }

    public bool IsPokemonValid(Pokemon pokemon)
    {
        return _pokemonRules.All(rule => rule.IsValid(pokemon));
    }

    public bool HasRegisterableTeam(IReadOnlyList<Pokemon> team)
    {
        return team.Count >= MinTeamLength && team.EachCombination(MaxTeamLength).Any(CanRegisterTeam);
    }

    public bool CanRegisterTeam(IReadOnlyList<Pokemon> team)
    {
        if (team.Count < MinTeamLength)
            return false;
        if (team.Count > MaxTeamLength)
            return false;

        var teamNumber = MinTeamLength;
        if (team.Any(pokemon => !IsPokemonValid(pokemon)))
        {
            return false;
        }

        if (_teamRules.Any(rule => !rule.IsValid(team)))
        {
            return false;
        }

        if (_subsetRules.Count <= 0)
            return true;

        return team.EachCombination(teamNumber)
                .Select(combination => _subsetRules.All(rule => rule.IsValid(combination)))
                .Any(isValid => isValid) || true;
    }

    public bool HasValidTeam(IReadOnlyList<Pokemon> team)
    {
        if (team.Count < MinTeamLength)
            return false;

        var teamNumber = MinTeamLength;
        var validPokemon = team.Where(IsPokemonValid).ToList();

        if (validPokemon.Count < teamNumber)
            return false;

        return _teamRules.Count <= 0 || team.EachCombination(teamNumber).Any(combination => IsValid(combination));
    }

    private static readonly Text ChoosePokemon = Text.Localized(
        "PokemonChallengeRuleset",
        "ChoosePokemon",
        "Choose a Pokémon."
    );
    private static readonly Text NumberNeeded = Text.Localized(
        "PokemonChallengeRuleset",
        "NumberNeeded",
        "{0} Pokémon are needed."
    );
    private static readonly Text NoMoreAllowed = Text.Localized(
        "PokemonChallengeRuleset",
        "NoMoreAllowed",
        "No more than {0} Pokémon may enter."
    );
    private static readonly Text PokemonNotAllowed = Text.Localized(
        "PokemonChallengeRuleset",
        "PokemonNotAllowed",
        "{0} is not allowed."
    );

    public bool IsValid(IReadOnlyList<Pokemon> team, ICollection<Text>? errors = null)
    {
        if (team.Count < MinLength)
        {
            if (errors is not null && MinLength == 1)
            {
                errors.Add(ChoosePokemon);
            }

            if (errors is not null && MinLength > 1)
            {
                errors.Add(Text.Format(NumberNeeded, MinLength));
            }

            return false;
        }

        if (team.Count > MaxLength)
        {
            errors?.Add(Text.Format(NoMoreAllowed, MaxLength));
            return false;
        }

        foreach (var pokemon in team)
        {
            if (IsPokemonValid(pokemon))
                continue;

            errors?.Add(Text.Format(PokemonNotAllowed, pokemon.Name));
            return false;
        }

        foreach (var rule in _teamRules.Where(rule => !rule.IsValid(team)))
        {
            errors?.Add(rule.ErrorMessage);
            return false;
        }

        foreach (var rule in _subsetRules.Where(rule => !rule.IsValid(team)))
        {
            errors?.Add(rule.ErrorMessage);
            return false;
        }

        return true;
    }
}
