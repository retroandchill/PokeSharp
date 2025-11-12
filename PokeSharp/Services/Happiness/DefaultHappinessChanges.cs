using Injectio.Attributes;
using PokeSharp.Abstractions;
using PokeSharp.Core.Settings;
using PokeSharp.Core.State;
using PokeSharp.Game;

namespace PokeSharp.Services.Happiness;

[RegisterSingleton]
public class ObtainMapHappinessChangeAdjuster : IHappinessChangeAdjuster
{
    public int Priority => 0;

    public int AdjustHappinessChange(Pokemon pokemon, HappinessChangeMethod method, int change)
    {
        if (change > 0 && pokemon.ObtainMap == GameMap.Instance.MapId)
        {
            return change + 1;
        }

        return change;
    }
}

[RegisterSingleton]
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

[RegisterSingleton]
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

[RegisterSingleton]
public class SoftCapHappinessChangeAdjuster : IHappinessChangeAdjuster
{
    public int Priority => 1000;

    public int AdjustHappinessChange(Pokemon pokemon, HappinessChangeMethod method, int change)
    {
        if (change > 0 && GameSettings.Instance.ApplyHappinessSoftCap && method.Id != HappinessChangeMethod.EVBerry.Id)
        {
            return change * 3 / 2;
        }

        return change;
    }
}
