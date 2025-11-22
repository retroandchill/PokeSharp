using System.Collections.Immutable;
using PokeSharp.Core;
using PokeSharp.Core.Utils;
using PokeSharp.Data.Core;
using PokeSharp.PokemonModel;
using PokeSharp.Settings;

namespace PokeSharp.BattleSystem.Challenge;

public record PokemonChallengeRuleset
{
    private readonly int _number;

    public PokemonChallengeRuleset(int number = 0)
    {
        _number = number;
    }

    public int Number
    {
        get => _number;
        init => NumberRange = (value, value);
    }

    public ImmutableArray<IPokemonRestriction> PokemonRules { get; init; } = [];
    public ImmutableArray<ITeamRestriction> TeamRules { get; init; } = [];
    public ImmutableArray<ITeamRestriction> SubsetRules { get; init; } = [];

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
            foreach (var rule in PokemonRules)
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
            foreach (var rule in SubsetRules.OfType<TotalLevelRestriction>())
            {
                totalLevel = rule.Level;
            }

            return totalLevel >= maxLevel * num
                ? Math.Max(maxLevel, minLevel)
                : Math.Max(totalLevel / SuggestedNumber, minLevel);
        }
    }

    public (int Min, int Max) NumberRange
    {
        init
        {
            MinLength = Math.Max(1, value.Min);
            _number = Math.Max(1, value.Max);
        }
    }

    public bool IsPokemonValid(Pokemon pokemon)
    {
        return PokemonRules.All(rule => rule.IsValid(pokemon));
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

        if (TeamRules.Any(rule => !rule.IsValid(team)))
        {
            return false;
        }

        if (SubsetRules.Length <= 0)
            return true;

        return team.EachCombination(teamNumber)
                .Select(combination => SubsetRules.All(rule => rule.IsValid(combination)))
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

        return TeamRules.Length <= 0 || team.EachCombination(teamNumber).Any(combination => IsValid(combination));
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

        foreach (var rule in TeamRules.Where(rule => !rule.IsValid(team)))
        {
            errors?.Add(rule.ErrorMessage);
            return false;
        }

        foreach (var rule in SubsetRules.Where(rule => !rule.IsValid(team)))
        {
            errors?.Add(rule.ErrorMessage);
            return false;
        }

        return true;
    }
}
