using Microsoft.Extensions.Options;

namespace PokeSharp.Core.Settings;

public record GameSettings
{
    public int MaxLevel { get; init; } = 100;

    public int ShinyChance { get; init; } = 16;

    public bool ApplyHappinessSoftCap { get; init; } = true;

    public bool DisableIVsAndEVs { get; init; } = false;
}

public static class GameSettingsServiceShortcut
{
    private static CachedService<IOptionsMonitor<GameSettings>> _cachedService;

    extension(GameServices)
    {
        public static GameSettings GameSettings => _cachedService.Instance.CurrentValue;
    }
}
