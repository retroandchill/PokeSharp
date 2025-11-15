using Microsoft.Extensions.Options;

namespace PokeSharp.Core.Settings;

public record GameSettings
{
    public int MaxLevel { get; init; } = 100;

    public int MaxPartySize { get; init; } = 6;

    public int ShinyChance { get; init; } = 16;

    public int StartMoney { get; init; } = 3000;

    public int MaxMoney { get; init; }

    public int MaxCoins { get; init; }

    public int MaxBattlePoints { get; init; }

    public int MaxSoot { get; init; }

    public bool ApplyHappinessSoftCap { get; init; } = true;

    public bool DisableIVsAndEVs { get; init; } = false;

    public bool UseCurrentRegionDex { get; init; } = true;
}

public static class GameSettingsServiceShortcut
{
    private static CachedService<IOptionsMonitor<GameSettings>> _cachedService;

    extension(GameServices)
    {
        public static GameSettings GameSettings => _cachedService.Instance.CurrentValue;
    }
}
