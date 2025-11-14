using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Core.Settings;

[AutoServiceShortcut]
public class GameSettings
{
    public int MaxLevel { get; init; } = 100;

    public int ShinyChance { get; init; } = 16;

    public bool ApplyHappinessSoftCap { get; init; } = true;

    public bool DisableIVsAndEVs { get; init; } = false;
}
