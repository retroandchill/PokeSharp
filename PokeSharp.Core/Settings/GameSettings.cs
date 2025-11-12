namespace PokeSharp.Core.Settings;

public class GameSettings
{
    public static GameSettings Instance { get; } = new();

    public int MaxLevel { get; init; } = 100;

    public int ShinyChance { get; init; } = 16;

    public bool ApplyHappinessSoftCap { get; init; } = true;

    public bool DisableIVsAndEVs { get; init; } = false;
}
