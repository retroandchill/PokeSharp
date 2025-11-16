using Injectio.Attributes;
using Microsoft.Extensions.Options;
using PokeSharp.Abstractions;
using PokeSharp.State;
using PokeSharp.PokemonModel;
using PokeSharp.Settings;

namespace PokeSharp.Services.Happiness;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class ObtainMapHappinessChangeAdjuster(GameMap gameMap) : IHappinessChangeAdjuster
{
    public int Priority => 0;

    public int AdjustHappinessChange(Pokemon pokemon, HappinessChangeMethod method, int change)
    {
        if (change > 0 && pokemon.ObtainMap == gameMap.MapId)
        {
            return change + 1;
        }

        return change;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class LuxuryBallHappinessChangeAdjuster : IHappinessChangeAdjuster
{
    private static readonly Name LuxuryBall = "LUXURYBALL";

    public int Priority => 10;

    public int AdjustHappinessChange(Pokemon pokemon, HappinessChangeMethod method, int change)
    {
        if (change > 0 && pokemon.PokeBall == LuxuryBall)
        {
            return change + 1;
        }

        return change;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class SootheBellHappinessChangeAdjuster : IHappinessChangeAdjuster
{
    private static readonly Name SootheBell = "SOOTHEBELL";

    public int Priority => 20;

    public int AdjustHappinessChange(Pokemon pokemon, HappinessChangeMethod method, int change)
    {
        if (change > 0 && pokemon.HasSpecificItem(SootheBell))
        {
            return change * 3 / 2;
        }

        return change;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class SoftCapHappinessChangeAdjuster(IOptionsMonitor<GameSettings> gameSettings) : IHappinessChangeAdjuster
{
    public int Priority => 1000;

    public int AdjustHappinessChange(Pokemon pokemon, HappinessChangeMethod method, int change)
    {
        if (
            change > 0
            && gameSettings.CurrentValue.ApplyHappinessSoftCap
            && method.Id != HappinessChangeMethod.EVBerry.Id
        )
        {
            return change * 3 / 2;
        }

        return change;
    }
}
