using Injectio.Attributes;
using PokeSharp.Abstractions;
using PokeSharp.Core;
using PokeSharp.Core.Settings;
using PokeSharp.Core.State;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using PokeSharp.Game;
using PokeSharp.Game.Items;
using PokeSharp.Services.DayNightCycle;
using PokeSharp.Services.Overworld;
using PokeSharp.Utilities;

namespace PokeSharp.Services.Evolution;

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Level.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelMaleEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelMale.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemon.IsMale;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelFemaleEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelFemale.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemon.IsFemale;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelDayEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelDay.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && GameServices.DayNightService.IsDay;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelNightEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelNight.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && GameServices.DayNightService.IsNight;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelMorningEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelMorning.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && GameServices.DayNightService.IsMorning;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelAfternoonEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelAfternoon.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && GameServices.DayNightService.IsAfternoon;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelEveningEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelEvening.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && GameServices.DayNightService.IsEvening;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelNoWeatherEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelNoWeather.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && GameServices.OverworldWeatherService.WeatherType.IsNone;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelSunEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelSun.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter
            && GameServices.OverworldWeatherService.Weather?.Category == BattleWeather.Sun.Id;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelRainEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelRain.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter
            && GameServices.OverworldWeatherService.Weather?.Category == BattleWeather.Rain.Id;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelSnowEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelSnow.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter
            && GameServices.OverworldWeatherService.Weather?.Category == BattleWeather.Hail.Id;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelSandstormEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelSandstorm.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter
            && GameServices.OverworldWeatherService.Weather?.Category == BattleWeather.Sandstorm.Id;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelCyclingEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelCycling.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && GameServices.PokemonGlobal.IsCycling;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelSurfingEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelSurfing.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && GameServices.PokemonGlobal.IsSurfing;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelDivingEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelDiving.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && GameServices.PokemonGlobal.IsDiving;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelDarknessEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelDarkness.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && GameServices.GameMap.HasMetadataTag(MapMetadataTags.DarkMap);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LevelDarkInPartyEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LevelDarkInParty.Id;
    private static readonly Name DarkType = "DARK";

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && PlayerTrainer.Instance.HasPokemonOfType(DarkType);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class AttackGreaterEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.AttackGreater.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemon.Attack > pokemon.Defense;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class AtkDefEqualEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.AtkDefEqual.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemon.Attack == pokemon.Defense;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class DefenseGreaterEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.DefenseGreater.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && pokemon.Attack < pokemon.Defense;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class SilcoonEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Silcoon.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && ((pokemon.PersonalityValue >> 16) & 0xFFFF) % 10 < 5;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class CascoonEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Cascoon.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter && ((pokemon.PersonalityValue >> 16) & 0xFFFF) % 10 >= 5;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class NinjaskEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Ninjask.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Level >= parameter;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class ShedinjaEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Shedinja.Id;
    private static readonly Name PokeBall = "POKEBALL";

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, int parameter, Name newSpecies)
    {
        if (PlayerTrainer.Instance.IsPartyFull || !Bag.Instance.HasItem(PokeBall))
            return false;

        pokemon.DuplicateForEvolution(newSpecies);
        Bag.Instance.RemoveItem(PokeBall);

        return true;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HappinessEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.Happiness.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= (GameServices.GameSettings.ApplyHappinessSoftCap ? 160 : 220);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HappinessMaleEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessMale.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= (GameServices.GameSettings.ApplyHappinessSoftCap ? 160 : 220) && pokemon.IsMale;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HappinessFemaleEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessFemale.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= (GameServices.GameSettings.ApplyHappinessSoftCap ? 160 : 220) && pokemon.IsFemale;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HappinessDayEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessDay.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= (GameServices.GameSettings.ApplyHappinessSoftCap ? 160 : 220)
            && GameServices.DayNightService.IsDay;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HappinessNightEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessNight.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= (GameServices.GameSettings.ApplyHappinessSoftCap ? 160 : 220)
            && GameServices.DayNightService.IsNight;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HappinessMoveEvolutionEvaluator : EvolutionMethodEvaluator<Name, Move>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessMove.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.Happiness >= (GameServices.GameSettings.ApplyHappinessSoftCap ? 160 : 220)
            && pokemon.Moves.Any(m => m.Id == parameter);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HappinessMoveTypeEvolutionEvaluator : EvolutionMethodEvaluator<Name, PokemonType>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessMoveType.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.Happiness >= (GameServices.GameSettings.ApplyHappinessSoftCap ? 160 : 220)
            && pokemon.Moves.Any(m => m.Type == parameter);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HappinessHoldItemEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HappinessHoldItem.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.Happiness >= (GameServices.GameSettings.ApplyHappinessSoftCap ? 160 : 220)
            && pokemon.HasSpecificItem(parameter);
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class MaxHappinessEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.MaxHappiness.Id;

    protected override bool OnLevelUp(Pokemon pokemon)
    {
        return pokemon.Happiness >= Pokemon.MaxHappiness;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class BeautyEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Beauty.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return pokemon.Beauty >= parameter;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
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

[RegisterSingleton<IEvolutionMethodEvaluator>]
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

[RegisterSingleton<IEvolutionMethodEvaluator>]
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

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class DayHoldItemEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.DayHoldItem.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.HasSpecificItem(parameter) && GameServices.DayNightService.IsDay;
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class NightHoldItemEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.NightHoldItem.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.HasSpecificItem(parameter) && GameServices.DayNightService.IsNight;
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HoldItemHappinessEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HoldItemHappiness.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.HasSpecificItem(parameter)
            && pokemon.Happiness >= (GameServices.GameSettings.ApplyHappinessSoftCap ? 160 : 220);
    }

    protected override bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name parameter, Name newSpecies)
    {
        return pokemon.RemoveHoldItemAfterEvolution(evoSpecies, parameter, newSpecies);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HasMoveEvolutionEvaluator : EvolutionMethodEvaluator<Name, Move>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HasMove.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.Moves.Any(m => m.Id == parameter);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HasMoveTypeEvolutionEvaluator : EvolutionMethodEvaluator<Name, PokemonType>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HasMoveType.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return pokemon.Moves.Any(m => m.Type == parameter);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class HasInPartyEvolutionEvaluator : EvolutionMethodEvaluator<SpeciesForm, Species>
{
    public override Name EvolutionMethod => Data.Core.Evolution.HasInParty.Id;

    protected override bool OnLevelUp(Pokemon pokemon, SpeciesForm parameter)
    {
        return PlayerTrainer.Instance.HasSpecies(parameter.Species);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LocationEvolutionEvaluator : EvolutionMethodEvaluator<Name>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Location.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return GameServices.GameMap.MapId == parameter;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class LocationFlagEvolutionEvaluator : EvolutionMethodEvaluator<Name>
{
    public override Name EvolutionMethod => Data.Core.Evolution.LocationFlag.Id;

    protected override bool OnLevelUp(Pokemon pokemon, Name parameter)
    {
        return GameServices.GameMap.HasMetadataTag(parameter);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class RegionEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Region.Id;

    protected override bool OnLevelUp(Pokemon pokemon, int parameter)
    {
        return GameServices.GameMap.RegionId == parameter;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class ItemEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Item.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class ItemMaleEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.ItemMale.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter && pokemon.IsMale;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class ItemFemaleEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.ItemFemale.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter && pokemon.IsFemale;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class ItemDayEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.ItemDay.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter && GameServices.DayNightService.IsDay;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class ItemNightEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.ItemNight.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter && GameServices.DayNightService.IsNight;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class ItemHappinessEvolutionEvaluator : EvolutionMethodEvaluator<Name, Item>
{
    public override Name EvolutionMethod => Data.Core.Evolution.ItemHappiness.Id;

    protected override bool OnUseItem(Pokemon pokemon, Name parameter, Name itemUsed)
    {
        return itemUsed == parameter
            && pokemon.Happiness >= (GameServices.GameSettings.ApplyHappinessSoftCap ? 160 : 220);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class TradeEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.Trade.Id;

    protected override bool OnTrade(Pokemon pokemon, Pokemon otherPokemon)
    {
        return true;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class TradeMaleEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.TradeMale.Id;

    protected override bool OnTrade(Pokemon pokemon, Pokemon otherPokemon)
    {
        return pokemon.IsMale;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class TradeFemaleEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.TradeFemale.Id;

    protected override bool OnTrade(Pokemon pokemon, Pokemon otherPokemon)
    {
        return pokemon.IsFemale;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class TradeDayEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.TradeDay.Id;

    protected override bool OnTrade(Pokemon pokemon, Pokemon otherPokemon)
    {
        return GameServices.DayNightService.IsDay;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class TradeNightEvolutionEvaluator : EvolutionMethodEvaluator
{
    public override Name EvolutionMethod => Data.Core.Evolution.TradeNight.Id;

    protected override bool OnTrade(Pokemon pokemon, Pokemon otherPokemon)
    {
        return GameServices.DayNightService.IsNight;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
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

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class TradeSpeciesEvolutionEvaluator : EvolutionMethodEvaluator<SpeciesForm, Species>
{
    public override Name EvolutionMethod => Data.Core.Evolution.TradeSpecies.Id;
    private static readonly Name Everstone = "EVERSTONE";

    protected override bool OnTrade(Pokemon pokemon, SpeciesForm parameter, Pokemon otherPokemon)
    {
        return pokemon.Species == parameter.Species && !otherPokemon.HasSpecificItem(Everstone);
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class BattleDealCriticalHitEvolutionEvaluator : EvolutionMethodEvaluator<int>
{
    public override Name EvolutionMethod => Data.Core.Evolution.BattleDealCriticalHit.Id;

    protected override bool AfterBattle(Pokemon pokemon, int partyIndex, int parameter)
    {
        return GameServices.GameTemp.PartyCriticalHitsDealt.Count > partyIndex
            && GameServices.GameTemp.PartyCriticalHitsDealt[partyIndex] >= parameter;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class EventEvolutionEvaluator : EvolutionMethodEvaluator<Name>
{
    public override Name EvolutionMethod => Data.Core.Evolution.Event.Id;

    protected override bool OnEvent(Pokemon pokemon, Name parameter, Name value)
    {
        return parameter == value;
    }
}

[RegisterSingleton<IEvolutionMethodEvaluator>]
public sealed class EventAfterDamageTakenEvolutionEvaluator : EvolutionMethodEvaluator<Name>
{
    public override Name EvolutionMethod => Data.Core.Evolution.EventAfterDamageTaken.Id;

    protected override bool AfterBattle(Pokemon pokemon, int partyIndex, Name parameter)
    {
        if (
            GameServices.GameTemp.PartyCriticalHitsDealt.Count > partyIndex
            && GameServices.GameTemp.PartyCriticalHitsDealt[partyIndex] >= 49
        )
        {
            pokemon.ReadyToEvolve = true;
        }

        return false;
    }

    protected override bool OnEvent(Pokemon pokemon, Name parameter, Name value)
    {
        return parameter == value && pokemon.ReadyToEvolve;
    }
}
