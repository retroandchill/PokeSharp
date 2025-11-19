using Injectio.Attributes;
using Microsoft.Extensions.Options;
using PokeSharp.Core;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using PokeSharp.Items;
using PokeSharp.PokemonModel;
using PokeSharp.Services.DayNightCycle;
using PokeSharp.Services.Overworld;
using PokeSharp.Settings;
using PokeSharp.State;
using PokeSharp.Trainers;
using PokeSharp.Utilities;
using Retro.ReadOnlyParams.Annotations;

namespace PokeSharp.Services.Evolution;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Level.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelMaleEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelMale.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemon.IsMale;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelFemaleEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelFemale.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemon.IsFemale;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelDayEvolutionEvaluator([ReadOnly] DayNightService dayNightService)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelDay.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && dayNightService.IsDay;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelNightEvolutionEvaluator([ReadOnly] DayNightService dayNightService)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelNight.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && dayNightService.IsNight;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelMorningEvolutionEvaluator([ReadOnly] DayNightService dayNightService)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelMorning.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && dayNightService.IsMorning;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelAfternoonEvolutionEvaluator([ReadOnly] DayNightService dayNightService)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelAfternoon.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && dayNightService.IsAfternoon;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelEveningEvolutionEvaluator([ReadOnly] DayNightService dayNightService)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelEvening.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && dayNightService.IsEvening;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelNoWeatherEvolutionEvaluator([ReadOnly] OverworldWeatherService overworldWeatherService)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelNoWeather.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && overworldWeatherService.WeatherType.IsNone;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelSunEvolutionEvaluator([ReadOnly] OverworldWeatherService overworldWeatherService)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelSun.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && overworldWeatherService.Weather?.Category == BattleWeather.Sun.Id;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelRainEvolutionEvaluator([ReadOnly] OverworldWeatherService overworldWeatherService)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelRain.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && overworldWeatherService.Weather?.Category == BattleWeather.Rain.Id;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelSnowEvolutionEvaluator([ReadOnly] OverworldWeatherService overworldWeatherService)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelSnow.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && overworldWeatherService.Weather?.Category == BattleWeather.Hail.Id;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelSandstormEvolutionEvaluator([ReadOnly] OverworldWeatherService overworldWeatherService)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelSandstorm.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && overworldWeatherService.Weather?.Category == BattleWeather.Sandstorm.Id;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelCyclingEvolutionEvaluator([ReadOnly] PokemonGlobal pokemonGlobal)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelCycling.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemonGlobal.IsCycling;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelSurfingEvolutionEvaluator([ReadOnly] PokemonGlobal pokemonGlobal)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelSurfing.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemonGlobal.IsSurfing;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelDivingEvolutionEvaluator([ReadOnly] PokemonGlobal pokemonGlobal)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelDiving.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemonGlobal.IsDiving;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelDarknessEvolutionEvaluator([ReadOnly] GameMap gameMap) : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelDarkness.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && gameMap.HasMetadataTag(MapMetadataTags.DarkMap);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LevelDarkInPartyEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelDarkInParty.Id;
    private static readonly Name DarkType = "DARK";

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && PlayerTrainer.Instance.HasPokemonOfType(DarkType);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class AttackGreaterEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.AttackGreater.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemon.Attack > pokemon.Defense;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class AtkDefEqualEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.AtkDefEqual.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemon.Attack == pokemon.Defense;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class DefenseGreaterEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.DefenseGreater.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemon.Attack < pokemon.Defense;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class SilcoonEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Silcoon.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && ((pokemon.PersonalityValue >> 16) & 0xFFFF) % 10 < 5;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class CascoonEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Cascoon.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && ((pokemon.PersonalityValue >> 16) & 0xFFFF) % 10 >= 5;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class NinjaskEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Ninjask.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class ShedinjaEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Shedinja.Id;
    private static readonly Name PokeBall = "POKEBALL";

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, int parameter, Name newSpecies)
    {
        if (PlayerTrainer.Instance.IsPartyFull || !PokemonBag.Instance.Has(PokeBall))
            return false;

        pokemon.DuplicateForEvolution(newSpecies);
        PokemonBag.Instance.Remove(PokeBall);

        return true;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HappinessEvolutionEvaluator([ReadOnly] IOptionsMonitor<GameSettings> gameSettings)
    : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.Happiness.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= (gameSettings.CurrentValue.ApplyHappinessSoftCap ? 160 : 220);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HappinessMaleEvolutionEvaluator([ReadOnly] IOptionsMonitor<GameSettings> gameSettings)
    : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessMale.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= (gameSettings.CurrentValue.ApplyHappinessSoftCap ? 160 : 220) && pokemon.IsMale;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HappinessFemaleEvolutionEvaluator([ReadOnly] IOptionsMonitor<GameSettings> gameSettings)
    : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessFemale.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= (gameSettings.CurrentValue.ApplyHappinessSoftCap ? 160 : 220) && pokemon.IsFemale;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HappinessDayEvolutionEvaluator(
    [ReadOnly] IOptionsMonitor<GameSettings> gameSettings,
    [ReadOnly] DayNightService dayNightService
) : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessDay.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= (gameSettings.CurrentValue.ApplyHappinessSoftCap ? 160 : 220)
            && dayNightService.IsDay;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HappinessNightEvolutionEvaluator(
    [ReadOnly] IOptionsMonitor<GameSettings> gameSettings,
    [ReadOnly] DayNightService dayNightService
) : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessNight.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= (gameSettings.CurrentValue.ApplyHappinessSoftCap ? 160 : 220)
            && dayNightService.IsNight;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HappinessMoveEvolutionEvaluator([ReadOnly] IOptionsMonitor<GameSettings> gameSettings)
    : EvolutionMethodEvaluator<Name, Move>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessMove.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.Happiness >= (gameSettings.CurrentValue.ApplyHappinessSoftCap ? 160 : 220)
            && pokemon.Moves.Any(m => m.Id == parameter);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HappinessMoveTypeEvolutionEvaluator([ReadOnly] IOptionsMonitor<GameSettings> gameSettings)
    : EvolutionMethodEvaluator<Name, PokemonType>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessMoveType.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.Happiness >= (gameSettings.CurrentValue.ApplyHappinessSoftCap ? 160 : 220)
            && pokemon.Moves.Any(m => m.Type == parameter);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HappinessHoldItemEvolutionEvaluator([ReadOnly] IOptionsMonitor<GameSettings> gameSettings)
    : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessHoldItem.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.Happiness >= (gameSettings.CurrentValue.ApplyHappinessSoftCap ? 160 : 220)
            && pokemon.HasSpecificItem(parameter);
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class MaxHappinessEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.MaxHappiness.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= Pokemon.MaxHappiness;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class BeautyEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Beauty.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Beauty >= parameter;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HoldItemEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HoldItem.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.HasSpecificItem(parameter);
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HoldItemMaleEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HoldItemMale.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.HasSpecificItem(parameter) && pokemon.IsMale;
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HoldItemFemaleEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HoldItemFemale.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.HasSpecificItem(parameter) && pokemon.IsFemale;
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class DayHoldItemEvolutionEvaluator([ReadOnly] DayNightService dayNightService)
    : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.DayHoldItem.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.HasSpecificItem(parameter) && dayNightService.IsDay;
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class NightHoldItemEvolutionEvaluator([ReadOnly] DayNightService dayNightService)
    : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.NightHoldItem.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.HasSpecificItem(parameter) && dayNightService.IsNight;
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HoldItemHappinessEvolutionEvaluator([ReadOnly] IOptionsMonitor<GameSettings> gameSettings)
    : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HoldItemHappiness.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.HasSpecificItem(parameter)
            && pokemon.Happiness >= (gameSettings.CurrentValue.ApplyHappinessSoftCap ? 160 : 220);
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HasMoveEvolutionEvaluator : EvolutionMethodEvaluator<Name, Move>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HasMove.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.Moves.Any(m => m.Id == parameter);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HasMoveTypeEvolutionEvaluator : EvolutionMethodEvaluator<Name, PokemonType>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HasMoveType.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.Moves.Any(m => m.Type == parameter);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HasInPartyEvolutionEvaluator : EvolutionMethodEvaluator<SpeciesForm, Species>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HasInParty.Id;

    protected override bool OnLevelUp(Pokemon pokemon, SpeciesForm parameter)
    {
        return PlayerTrainer.Instance.HasSpecies(parameter.Species);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LocationEvolutionEvaluator([ReadOnly] GameMap gameMap) : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Location.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return gameMap.MapId == parameter;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class LocationFlagEvolutionEvaluator([ReadOnly] GameMap gameMap) : EvolutionMethodEvaluator<Name>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LocationFlag.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return gameMap.HasMetadataTag(parameter);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class RegionEvolutionEvaluator([ReadOnly] GameMap gameMap) : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Region.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return gameMap.RegionId == parameter;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class ItemEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Item.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class ItemMaleEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.ItemMale.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter && pokemon.IsMale;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class ItemFemaleEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.ItemFemale.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter && pokemon.IsFemale;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class ItemDayEvolutionEvaluator([ReadOnly] DayNightService dayNightService)
    : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.ItemDay.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter && dayNightService.IsDay;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class ItemNightEvolutionEvaluator([ReadOnly] DayNightService dayNightService)
    : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.ItemNight.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter && dayNightService.IsNight;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class ItemHappinessEvolutionEvaluator([ReadOnly] IOptionsMonitor<GameSettings> gameSettings)
    : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.ItemHappiness.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter
            && pokemon.Happiness >= (gameSettings.CurrentValue.ApplyHappinessSoftCap ? 160 : 220);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class TradeEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.Trade.Id;

    protected override bool OnTrade(Pokemon pokemon, Pokemon otherPokemon)
    {
        return true;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class TradeMaleEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.TradeMale.Id;

    protected override bool OnTrade(Pokemon pokemon, Pokemon otherPokemon)
    {
        return pokemon.IsMale;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class TradeFemaleEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.TradeFemale.Id;

    protected override bool OnTrade(Pokemon pokemon, Pokemon otherPokemon)
    {
        return pokemon.IsFemale;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class TradeDayEvolutionEvaluator([ReadOnly] DayNightService dayNightService) : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.TradeDay.Id;

    protected override bool OnTrade(Pokemon pokemon, Pokemon otherPokemon)
    {
        return dayNightService.IsDay;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class TradeNightEvolutionEvaluator([ReadOnly] DayNightService dayNightService) : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.TradeNight.Id;

    protected override bool OnTrade(Pokemon pokemon, Pokemon otherPokemon)
    {
        return dayNightService.IsNight;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class TradeItemEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.TradeItem.Id;

    protected override bool OnTrade(Pokemon pokemon, Name parameter, Pokemon otherPokemon)
    {
        return pokemon.HasSpecificItem(parameter);
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class TradeSpeciesEvolutionEvaluator : EvolutionMethodEvaluator<SpeciesForm, Species>
{
    public override Name EvolutionMethod => Data.Core.Evolution.TradeSpecies.Id;
    private static readonly Name Everstone = "EVERSTONE";

    protected override bool OnTrade(Pokemon pokemon, SpeciesForm parameter, Pokemon otherPokemon)
    {
        return pokemon.Species == parameter.Species && !otherPokemon.HasSpecificItem(Everstone);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class BattleDealCriticalHitEvolutionEvaluator([ReadOnly] GameTemp gameTemp)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.BattleDealCriticalHit.Id;

    protected override bool AfterBattle(Pokemon pokemon, int partyIndex, int parameter)
    {
        return gameTemp.PartyCriticalHitsDealt.Count > partyIndex
            && gameTemp.PartyCriticalHitsDealt[partyIndex] >= parameter;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class EventEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Event.Id;

    protected override bool OnEvent(Pokemon pokemon, int parameter, int value)
    {
        return parameter == value;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class EventAfterDamageTakenEvolutionEvaluator([ReadOnly] GameTemp gameTemp)
    : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.EventAfterDamageTaken.Id;

    protected override bool AfterBattle(Pokemon pokemon, int partyIndex, int parameter)
    {
        if (gameTemp.PartyCriticalHitsDealt.Count > partyIndex && gameTemp.PartyCriticalHitsDealt[partyIndex] >= 49)
        {
            pokemon.AddTag(PokemonTags.ReadyToEvolve);
        }

        return false;
    }

    protected override bool OnEvent(Pokemon pokemon, int parameter, int value)
    {
        return parameter == value && pokemon.HasTag(PokemonTags.ReadyToEvolve);
    }
}
