using PokeSharp.Data.Core;

namespace PokeSharp.Data;

public static class DefaultData
{
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
