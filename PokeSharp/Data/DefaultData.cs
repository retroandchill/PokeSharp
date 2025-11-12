using PokeSharp.Data.Core;

namespace PokeSharp.Data;

/// <summary>
/// Contains all the default data for the game.
/// </summary>
public static class DefaultData
{
    /// <summary>
    /// Initializes all default data entities.
    /// </summary>
    public static void AddAll()
    {
        GrowthRate.AddDefaultValues();
        GenderRatio.AddDefaultValues();
        EggGroup.AddDefaultValues();
        BodyShape.AddDefaultValues();
        BodyColor.AddDefaultValues();
        Habitat.AddDefaultValues();
        Evolution.AddDefaultValues();
        Stat.AddDefaultValues();
        Nature.AddDefaultValues();
        Weather.AddDefaultValues();
        EncounterType.AddDefaultValues();
        GameEnvironment.AddDefaultValues();
        BattleWeather.AddDefaultValues();
        BattleTerrain.AddDefaultValues();
        Target.AddDefaultValues();
    }
}
