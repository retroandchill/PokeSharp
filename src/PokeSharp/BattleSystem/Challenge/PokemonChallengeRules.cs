using System.Collections.Immutable;
using PokeSharp.PokemonModel;
using PokeSharp.Trainers;

namespace PokeSharp.BattleSystem.Challenge;

public record PokemonChallengeRules
{
    public PokemonChallengeRuleset Ruleset { get; private init; }
    public BattleType BattleType { get; init; } = BattleTower.Default;
    public LevelAdjustment? LevelAdjustment { get; init; }
    public ImmutableArray<IBattleRule> BattleRules { get; init; } = [];

    public int Number
    {
        get => Ruleset.Number;
        init => Ruleset = Ruleset with { Number = value };
    }

    public bool DoubleBattle
    {
        init
        {
            if (value)
            {
                Ruleset = Ruleset with { Number = 4 };
                BattleRules = BattleRules.Add(new DoubleBattle());
            }
            else
            {
                Ruleset = Ruleset with { Number = 3 };
                BattleRules = BattleRules.Add(new SingleBattle());
            }
        }
    }

    public (int Min, int Max, int Total) LevelRule
    {
        init
        {
            Ruleset = Ruleset with
            {
                PokemonRules = Ruleset.PokemonRules.AddRange(
                    new MinimumLevelRestriction(value.Min),
                    new MaximumLevelRestriction(value.Max)
                ),
                SubsetRules = Ruleset.SubsetRules.Add(new TotalLevelRestriction(value.Total)),
            };
            LevelAdjustment = new TotalLevelAdjustment(value.Min, value.Max, value.Total);
        }
    }

    public PokemonChallengeRules(PokemonChallengeRuleset? ruleset = null)
    {
        Ruleset = ruleset ?? new PokemonChallengeRuleset();
    }

    public Adjustments? AdjustLevels(IReadOnlyList<Pokemon> team1, IReadOnlyList<Pokemon> team2)
    {
        return LevelAdjustment?.AdjustLevels(team1, team2);
    }

    public void UnadjustLevels(IReadOnlyList<Pokemon> team1, IReadOnlyList<Pokemon> team2, Adjustments? adjustments)
    {
        if (adjustments is not null && LevelAdjustment is not null)
        {
            LevelAdjustment.UnadjustLevels(team1, team2, adjustments.Value);
        }
    }

    public Adjustments? AdjustLevelsBilateral(IReadOnlyList<Pokemon> team1, IReadOnlyList<Pokemon> team2)
    {
        if (LevelAdjustment is not { Type: LevelAdjustmentType.BothTeams })
            return null;
        return LevelAdjustment.AdjustLevels(team1, team2);
    }

    public void UnadjustLevelsBilateral(
        IReadOnlyList<Pokemon> team1,
        IReadOnlyList<Pokemon> team2,
        Adjustments? adjustments
    )
    {
        if (adjustments is not null && LevelAdjustment is { Type: LevelAdjustmentType.BothTeams })
        {
            LevelAdjustment.UnadjustLevels(team1, team2, adjustments.Value);
        }
    }

    public Battle CreateBattle(IBattleScreen screen, Trainer trainer1, Trainer trainer2)
    {
        var battle = BattleType.CreateBattle(screen, trainer1, trainer2);
        foreach (var rule in BattleRules)
        {
            rule.SetRule(battle);
        }

        return battle;
    }
}
