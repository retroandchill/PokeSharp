using PokeSharp.PokemonModel;
using PokeSharp.Trainers;

namespace PokeSharp.BattleSystem.Challenge;

public class PokemonChallengeRules(PokemonChallengeRuleset? ruleset = null)
{
    public PokemonChallengeRuleset Ruleset { get; set; } = ruleset ?? new PokemonChallengeRuleset();
    public BattleType BattleType { get; set; } = new BattleTower();
    public LevelAdjustment? LevelAdjustment { get; set; }
    private readonly List<IBattleRule> _battleRules = [];

    public int Number
    {
        get => Ruleset.Number;
        set => Ruleset.Number = value;
    }

    public PokemonChallengeRules Copy()
    {
        var result = new PokemonChallengeRules(Ruleset.Copy())
        {
            BattleType = BattleType,
            LevelAdjustment = LevelAdjustment,
        };
        foreach (var rule in _battleRules)
        {
            result.AddBattleRule(rule);
        }

        return result;
    }

    public bool DoubleBattle
    {
        set
        {
            if (value)
            {
                Ruleset.Number = 4;
                AddBattleRule(new DoubleBattle());
            }
            else
            {
                Ruleset.Number = 3;
                AddBattleRule(new SingleBattle());
            }
        }
    }

    public Adjustments? AdjustLevels(IReadOnlyList<Pokemon> team1, IReadOnlyList<Pokemon> team2)
    {
        return LevelAdjustment?.AdjustLevels(team1, team2);
    }

    public void UnadjustLevels(IReadOnlyList<Pokemon> team1, IReadOnlyList<Pokemon> team2, Adjustments? adjustments)
    {
        if (adjustments is not null)
        {
            LevelAdjustment?.UnadjustLevels(team1, team2, adjustments.Value);
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

    public void AddPokemonRule(IPokemonRestriction restriction)
    {
        Ruleset.AddPokemonRule(restriction);
    }

    public void AddLevelRule(int minLevel, int maxLevel, int totalLevel)
    {
        AddPokemonRule(new MinimumLevelRestriction(minLevel));
        AddPokemonRule(new MaximumLevelRestriction(maxLevel));
        AddSubsetRule(new TotalLevelRestriction(totalLevel));
        LevelAdjustment = new TotalLevelAdjustment(minLevel, maxLevel, totalLevel);
    }

    public void AddSubsetRule(ITeamRestriction teamRestriction)
    {
        Ruleset.AddSubsetRule(teamRestriction);
    }

    public void AddTeamRule(ITeamRestriction teamRestriction)
    {
        Ruleset.AddTeamRule(teamRestriction);
    }

    public void AddBattleRule(IBattleRule rule)
    {
        _battleRules.Add(rule);
    }

    public Battle CreateBattle(IBattleScreen screen, Trainer trainer1, Trainer trainer2)
    {
        var battle = BattleType.CreateBattle(screen, trainer1, trainer2);
        foreach (var rule in _battleRules)
        {
            rule.SetRule(battle);
        }

        return battle;
    }
}
