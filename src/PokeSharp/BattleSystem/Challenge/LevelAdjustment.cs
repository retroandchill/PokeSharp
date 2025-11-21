using System.Collections.Immutable;
using PokeSharp.Data.Core;
using PokeSharp.PokemonModel;

namespace PokeSharp.BattleSystem.Challenge;

public enum LevelAdjustmentType : byte
{
    BothTeams,
    EnemyTeam,
    PlayerTeam,
    BothTeamsDifferent,
}

public readonly record struct Adjustments(ImmutableArray<int> Team1, ImmutableArray<int> Team2);

public class LevelAdjustment(LevelAdjustmentType type)
{
    public LevelAdjustmentType Type { get; } = type;

    private static IEnumerable<int> GetNullAdjustment(IEnumerable<Pokemon> team)
    {
        return team.Select(p => p.Level);
    }

    protected virtual IEnumerable<int> GetAdjustment(IEnumerable<Pokemon> team) => GetNullAdjustment(team);

    protected virtual IEnumerable<int> GetPlayerAdjustment(IEnumerable<Pokemon> team) => GetNullAdjustment(team);

    protected virtual IEnumerable<int> GetOpponentAdjustment(IEnumerable<Pokemon> team) => GetNullAdjustment(team);

    public IEnumerable<int> GetOldExp(IEnumerable<Pokemon> team) => team.Select(p => p.Exp);

    public void UnadjustLevels(IReadOnlyList<Pokemon> team1, IReadOnlyList<Pokemon> team2, Adjustments adjustments)
    {
        foreach (var (i, pokemon) in team1.Index())
        {
            if (adjustments.Team1.Length <= i || pokemon.Exp == adjustments.Team1[i])
                continue;

            pokemon.Exp = adjustments.Team1[i];
            pokemon.CalcStats();
        }

        foreach (var (i, pokemon) in team2.Index())
        {
            if (adjustments.Team2.Length <= i || pokemon.Exp == adjustments.Team2[i])
                continue;

            pokemon.Exp = adjustments.Team2[i];
            pokemon.CalcStats();
        }
    }

    public Adjustments AdjustLevels(IReadOnlyList<Pokemon> team1, IReadOnlyList<Pokemon> team2)
    {
        int[]? adjustment1 = null;
        int[]? adjustment2 = null;
        var result = new Adjustments([.. GetOldExp(team1)], [.. GetOldExp(team2)]);
        switch (Type)
        {
            case LevelAdjustmentType.BothTeams:
                adjustment1 = GetAdjustment(team1).ToArray();
                adjustment2 = GetAdjustment(team2).ToArray();
                break;
            case LevelAdjustmentType.EnemyTeam:
                adjustment2 = GetAdjustment(team2).ToArray();
                break;
            case LevelAdjustmentType.PlayerTeam:
                adjustment1 = GetAdjustment(team1).ToArray();
                break;
            case LevelAdjustmentType.BothTeamsDifferent:
                adjustment1 = GetPlayerAdjustment(team1).ToArray();
                adjustment2 = GetOpponentAdjustment(team2).ToArray();
                break;
            default:
                throw new InvalidOperationException("Unknown adjustment type");
        }

        AdjustTeamLevels(team1, adjustment1);

        AdjustTeamLevels(team2, adjustment2);

        return result;
    }

    private static void AdjustTeamLevels(IReadOnlyList<Pokemon> team, int[]? adjustment)
    {
        if (adjustment is null)
            return;
        foreach (var (i, pokemon) in team.Index())
        {
            if (pokemon.Level == adjustment[i])
                continue;

            pokemon.Level = adjustment[i];
            pokemon.CalcStats();
        }
    }
}

public class TotalLevelAdjustment(int minLevel, int maxLevel, int totalLevel)
    : LevelAdjustment(LevelAdjustmentType.EnemyTeam)
{
    private readonly int _minLevel = Math.Clamp(minLevel, 1, GrowthRate.MaxLevel);
    private readonly int _maxLevel = Math.Clamp(maxLevel, 1, GrowthRate.MaxLevel);
    private readonly int _totalLevel = Math.Clamp(totalLevel, 1, GrowthRate.MaxLevel);

    protected override IEnumerable<int> GetAdjustment(IEnumerable<Pokemon> team)
    {
        var teamList = team as IReadOnlyList<Pokemon> ?? team.ToList();

        var result = new List<int>();
        var total = 0;
        foreach (var pokemon in teamList)
        {
            result.Add(_minLevel);
            total += _minLevel;
        }

        while (true)
        {
            var work = false;
            for (var i = 0; i < teamList.Count; i++)
            {
                if (result[i] >= _maxLevel || total >= totalLevel)
                    continue;

                result[i]++;
                total++;
                work = true;
            }

            if (!work)
                break;
        }

        return result;
    }
}
