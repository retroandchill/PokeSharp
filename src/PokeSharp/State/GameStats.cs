using Injectio.Attributes;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core;
using PokeSharp.Core.State;
using PokeSharp.Data.Pbs;
using PokeSharp.Trainers;

namespace PokeSharp.State;

[AutoServiceShortcut(Type = AutoServiceShortcutType.GameState)]
[MessagePackObject(true)]
public class GameStats
{
    #region Travel

    public int DistanceWalked { get; set; }

    public int DistanceCycled { get; set; }

    public int DistanceSurfed { get; set; }

    public int DistanceMoved => DistanceWalked + DistanceCycled + DistanceSurfed;

    public int DistanceSlidOnIce { get; set; }

    public int BumpCount { get; set; }

    public int CycleCount { get; set; }

    public int SurfCount { get; set; }

    public int DiveCount { get; set; }

    #endregion

    #region Field actions

    public int FlyCount { get; set; }

    public int RockSmashCount { get; set; }

    public int RockSmashBattles { get; set; }

    public int HeadbuttCount { get; set; }

    public int HeadbuttBattles { get; set; }

    public int StrengthPushCount { get; set; }

    public int WaterfallCount { get; set; }

    public int WaterfallsDescended { get; set; }

    #endregion

    #region Items

    public int RepelCount { get; set; }

    public int ItemFinderCount { get; set; }

    public int FishingCount { get; set; }

    public int FishingBattles { get; set; }

    public int PokeRadarCount { get; set; }

    public int PokeRadarLongestChain { get; set; }

    public int BerryPlantsPicked { get; set; }

    public int MaxYieldBerryPlants { get; set; }

    public int BerriesPlanted { get; set; }

    #endregion

    #region NPCs

    public int PokeCenterCount { get; set; }

    public int RevivedFossilCount { get; set; }

    public int LotteryPrizeCount { get; set; }

    #endregion

    #region Pokémon

    public int CaughtPokemonCount
    {
        get
        {
            var player = GameGlobal.PlayerTrainer;
            return Species.AllSpecies.Aggregate(0, (c, sp) => c + player.Pokedex.GetCaughtCount(sp.SpeciesId));
        }
    }

    public int EggHatched { get; set; }

    public int EvolutionCount { get; set; }

    public int EvolutionsCancelled { get; set; }

    public int TradeCount { get; set; }

    public int MovesTaughtByItem { get; set; }

    public int MovesTaughtByTutor { get; set; }

    public int MovesTaughtByReminder { get; set; }

    public int DayCareDeposits { get; set; }

    public int DayCareLevelsGained { get; set; }

    public int PokerusInfections { get; set; }

    public int ShadowPokemonPurified { get; set; }

    #endregion

    #region Battles

    public int WildBattlesWon { get; set; }

    public int WildBattlesLost { get; set; }

    public int TrainerBattlesWon { get; set; }

    public int TrainerBattlesLost { get; set; }

    public int TotalExpGained { get; set; }

    public int BattleMoneyGained { get; set; }

    public int BattleMoneyLost { get; set; }

    public int BlackedOutCount { get; set; }

    public int MegaEvolutionCount { get; set; }

    public int FailedPokeBallCount { get; set; }

    #endregion

    #region Currency

    public int MoneySpendAtMarts { get; set; }

    public int MoneyEarnedAtMarts { get; set; }

    public int MartItemsBought { get; set; }

    public int PremierBallsEarned { get; set; }

    public int DrinksBought { get; set; }

    public int DrinksWon { get; set; }

    public int CoinsWon { get; set; }

    public int CoinsLost { get; set; }

    public int BattlePointsWon { get; set; }

    public int BattlePointsSpent { get; set; }

    public int SootCollected { get; set; }

    #endregion

    #region Special stats

    public Dictionary<int, int> GymLeaderAttempts { get; set; } = new();

    public Dictionary<int, float> TimesToGetBadges { get; set; } = new();

    public void SetTimeToBadge(int badgeId) => TimesToGetBadges[badgeId] = PlayTime;

    public int EliteFourAttempts { get; set; }

    public int HallOfFameEntryCount { get; set; }

    public float TimeToEnterHallOfFame { get; set; }

    public void SetTimeToEnterHallOfFame() => TimeToEnterHallOfFame = PlayTime;

    public int SafariPokemonCaught { get; set; }

    public int MostCapturesPerSafariGame { get; set; }

    public int BugContestCount { get; set; }

    public int BugContestWins { get; set; }

    #endregion

    #region Play

    public float PlayTime { get; set; }

    public int PlaySessions { get; set; }

    public float PlayTimePerSession => PlayTime / PlaySessions;

    public float TimeLastSaved { get; set; }

    public void SetTimeLastSaved() => TimeLastSaved = PlayTime;

    public float TimeSinceLastSave => PlayTime - TimeLastSaved;

    public int SaveCount => GameGlobal.GameSystem.SaveCount;

    #endregion
}

public static class GameStatsExtensions
{
    [RegisterServices]
    public static void RegisterGameStats(this IServiceCollection services)
    {
        services.AddGameState<GameStats>();
    }
}
