using Injectio.Attributes;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents an evolution step or method for a Pokémon entity, defining how it can evolve under specific conditions or actions.
/// </summary>
[GameDataEntity]
public partial record Evolution
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Gets the name representation associated with the evolution entity. This property is required and immutable.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// Represents a configurable parameter associated with the evolution data.
    /// </summary>
    public Type? Parameter { get; init; }

    /// <summary>
    /// Indicates whether any level-up condition is required for the evolution.
    /// </summary>
    public bool AnyLevelUp { get; init; }
}

[GameDataRegistration<Evolution>]
[RegisterSingleton<IGameDataProvider<Evolution>>]
public partial class EvolutionRegistrations
{
    private const string LocalizationNamespace = "GameData.Evolution";

    [GameDataEntityRegistration]
    internal static readonly Evolution Level = new()
    {
        Id = "Level",
        Name = Text.Localized(LocalizationNamespace, "Level", "Level"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelMale = new()
    {
        Id = "LevelMale",
        Name = Text.Localized(LocalizationNamespace, "LevelMale", "LevelMale"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelFemale = new()
    {
        Id = "LevelFemale",
        Name = Text.Localized(LocalizationNamespace, "LevelFemale", "LevelFemale"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelDay = new()
    {
        Id = "LevelDay",
        Name = Text.Localized(LocalizationNamespace, "LevelDay", "LevelDay"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelNight = new()
    {
        Id = "LevelNight",
        Name = Text.Localized(LocalizationNamespace, "LevelNight", "LevelNight"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelMorning = new()
    {
        Id = "LevelMorning",
        Name = Text.Localized(LocalizationNamespace, "LevelMorning", "LevelMorning"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelAfternoon = new()
    {
        Id = "LevelAfternoon",
        Name = Text.Localized(LocalizationNamespace, "LevelAfternoon", "LevelAfternoon"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelEvening = new()
    {
        Id = "LevelEvening",
        Name = Text.Localized(LocalizationNamespace, "LevelEvening", "LevelEvening"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelNoWeather = new()
    {
        Id = "LevelNoWeather",
        Name = Text.Localized(LocalizationNamespace, "LevelNoWeather", "LevelNoWeather"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelSun = new()
    {
        Id = "LevelSun",
        Name = Text.Localized(LocalizationNamespace, "LevelSun", "LevelSun"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelRain = new()
    {
        Id = "LevelRain",
        Name = Text.Localized(LocalizationNamespace, "LevelRain", "LevelRain"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelSnow = new()
    {
        Id = "LevelSnow",
        Name = Text.Localized(LocalizationNamespace, "LevelSnow", "LevelSnow"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelSandstorm = new()
    {
        Id = "LevelSandstorm",
        Name = Text.Localized(LocalizationNamespace, "LevelSandstorm", "LevelSandstorm"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelCycling = new()
    {
        Id = "LevelCycling",
        Name = Text.Localized(LocalizationNamespace, "LevelCycling", "LevelCycling"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelSurfing = new()
    {
        Id = "LevelSurfing",
        Name = Text.Localized(LocalizationNamespace, "LevelSurfing", "LevelSurfing"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelDiving = new()
    {
        Id = "LevelDiving",
        Name = Text.Localized(LocalizationNamespace, "LevelDiving", "LevelDiving"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelDarkness = new()
    {
        Id = "LevelDarkness",
        Name = Text.Localized(LocalizationNamespace, "LevelDarkness", "LevelDarkness"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LevelDarkInParty = new()
    {
        Id = "LevelDarkInParty",
        Name = Text.Localized(LocalizationNamespace, "LevelDarkInParty", "LevelDarkInParty"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution AttackGreater = new()
    {
        Id = "AttackGreater",
        Name = Text.Localized(LocalizationNamespace, "AttackGreater", "AttackGreater"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution AtkDefEqual = new()
    {
        Id = "AtkDefEqual",
        Name = Text.Localized(LocalizationNamespace, "AtkDefEqual", "AtkDefEqual"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution DefenseGreater = new()
    {
        Id = "DefenseGreater",
        Name = Text.Localized(LocalizationNamespace, "DefenseGreater", "DefenseGreater"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution Silcoon = new()
    {
        Id = "Silcoon",
        Name = Text.Localized(LocalizationNamespace, "Silcoon", "Silcoon"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution Cascoon = new()
    {
        Id = "Cascoon",
        Name = Text.Localized(LocalizationNamespace, "Cascoon", "Cascoon"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution Ninjask = new()
    {
        Id = "Ninjask",
        Name = Text.Localized(LocalizationNamespace, "Ninjask", "Ninjask"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution Shedinja = new()
    {
        Id = "Shedinja",
        Name = Text.Localized(LocalizationNamespace, "Shedinja", "Shedinja"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution Happiness = new()
    {
        Id = "Happiness",
        Name = Text.Localized(LocalizationNamespace, "Happiness", "Happiness"),
        AnyLevelUp = true,
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HappinessMale = new()
    {
        Id = "HappinessMale",
        Name = Text.Localized(LocalizationNamespace, "HappinessMale", "HappinessMale"),
        AnyLevelUp = true,
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HappinessFemale = new()
    {
        Id = "HappinessFemale",
        Name = Text.Localized(LocalizationNamespace, "HappinessFemale", "HappinessFemale"),
        AnyLevelUp = true,
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HappinessDay = new()
    {
        Id = "HappinessDay",
        Name = Text.Localized(LocalizationNamespace, "HappinessDay", "HappinessDay"),
        AnyLevelUp = true,
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HappinessNight = new()
    {
        Id = "HappinessNight",
        Name = Text.Localized(LocalizationNamespace, "HappinessNight", "HappinessNight"),
        AnyLevelUp = true,
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HappinessMove = new()
    {
        Id = "HappinessMove",
        Name = Text.Localized(LocalizationNamespace, "HappinessMove", "HappinessMove"),
        AnyLevelUp = true,
        Parameter = typeof(Move),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HappinessMoveType = new()
    {
        Id = "HappinessMoveType",
        Name = Text.Localized(LocalizationNamespace, "HappinessMoveType", "HappinessMoveType"),
        AnyLevelUp = true,
        Parameter = typeof(PokemonType),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HappinessHoldItem = new()
    {
        Id = "HappinessHoldItem",
        Name = Text.Localized(LocalizationNamespace, "HappinessHoldItem", "HappinessHoldItem"),
        AnyLevelUp = true,
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution MaxHappiness = new()
    {
        Id = "MaxHappiness",
        Name = Text.Localized(LocalizationNamespace, "MaxHappiness", "MaxHappiness"),
        AnyLevelUp = true,
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution Beauty = new()
    {
        Id = "Beauty",
        Name = Text.Localized(LocalizationNamespace, "Beauty", "Beauty"),
        AnyLevelUp = true,
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HoldItem = new()
    {
        Id = "HoldItem",
        Name = Text.Localized(LocalizationNamespace, "HoldItem", "HoldItem"),
        AnyLevelUp = true,
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HoldItemMale = new()
    {
        Id = "HoldItemMale",
        Name = Text.Localized(LocalizationNamespace, "HoldItemMale", "HoldItemMale"),
        AnyLevelUp = true,
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HoldItemFemale = new()
    {
        Id = "HoldItemFemale",
        Name = Text.Localized(LocalizationNamespace, "HoldItemFemale", "HoldItemFemale"),
        AnyLevelUp = true,
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution DayHoldItem = new()
    {
        Id = "DayHoldItem",
        Name = Text.Localized(LocalizationNamespace, "DayHoldItem", "DayHoldItem"),
        AnyLevelUp = true,
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution NightHoldItem = new()
    {
        Id = "NightHoldItem",
        Name = Text.Localized(LocalizationNamespace, "NightHoldItem", "NightHoldItem"),
        AnyLevelUp = true,
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HoldItemHappiness = new()
    {
        Id = "HoldItemHappiness",
        Name = Text.Localized(LocalizationNamespace, "HoldItemHappiness", "HoldItemHappiness"),
        AnyLevelUp = true,
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HasMove = new()
    {
        Id = "HasMove",
        Name = Text.Localized(LocalizationNamespace, "HasMove", "HasMove"),
        AnyLevelUp = true,
        Parameter = typeof(Move),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HasMoveType = new()
    {
        Id = "HasMoveType",
        Name = Text.Localized(LocalizationNamespace, "HasMoveType", "HasMoveType"),
        AnyLevelUp = true,
        Parameter = typeof(PokemonType),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution HasInParty = new()
    {
        Id = "HasInParty",
        Name = Text.Localized(LocalizationNamespace, "HasInParty", "HasInParty"),
        AnyLevelUp = true,
        Parameter = typeof(Species),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution Location = new()
    {
        Id = "Location",
        Name = Text.Localized(LocalizationNamespace, "Location", "Location"),
        AnyLevelUp = true,
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution LocationFlag = new()
    {
        Id = "LocationFlag",
        Name = Text.Localized(LocalizationNamespace, "LocationFlag", "LocationFlag"),
        AnyLevelUp = true,
        Parameter = typeof(Name),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution Region = new()
    {
        Id = "Region",
        Name = Text.Localized(LocalizationNamespace, "Region", "Region"),
        AnyLevelUp = true,
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution Item = new()
    {
        Id = "Item",
        Name = Text.Localized(LocalizationNamespace, "Item", "Item"),
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution ItemMale = new()
    {
        Id = "ItemMale",
        Name = Text.Localized(LocalizationNamespace, "ItemMale", "ItemMale"),
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution ItemFemale = new()
    {
        Id = "ItemFemale",
        Name = Text.Localized(LocalizationNamespace, "ItemFemale", "ItemFemale"),
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution ItemDay = new()
    {
        Id = "ItemDay",
        Name = Text.Localized(LocalizationNamespace, "ItemDay", "ItemDay"),
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution ItemNight = new()
    {
        Id = "ItemNight",
        Name = Text.Localized(LocalizationNamespace, "ItemNight", "ItemNight"),
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution ItemHappiness = new()
    {
        Id = "ItemHappiness",
        Name = Text.Localized(LocalizationNamespace, "ItemHappiness", "ItemHappiness"),
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution Trade = new()
    {
        Id = "Trade",
        Name = Text.Localized(LocalizationNamespace, "Trade", "Trade"),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution TradeMale = new()
    {
        Id = "TradeMale",
        Name = Text.Localized(LocalizationNamespace, "TradeMale", "TradeMale"),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution TradeFemale = new()
    {
        Id = "TradeFemale",
        Name = Text.Localized(LocalizationNamespace, "TradeFemale", "TradeFemale"),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution TradeDay = new()
    {
        Id = "TradeDay",
        Name = Text.Localized(LocalizationNamespace, "TradeDay", "TradeDay"),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution TradeNight = new()
    {
        Id = "TradeNight",
        Name = Text.Localized(LocalizationNamespace, "TradeNight", "TradeNight"),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution TradeItem = new()
    {
        Id = "TradeItem",
        Name = Text.Localized(LocalizationNamespace, "TradeItem", "TradeItem"),
        Parameter = typeof(Item),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution TradeSpecies = new()
    {
        Id = "TradeSpecies",
        Name = Text.Localized(LocalizationNamespace, "TradeSpecies", "TradeSpecies"),
        Parameter = typeof(Species),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution BattleDealCriticalHit = new()
    {
        Id = "BattleDealCriticalHit",
        Name = Text.Localized(LocalizationNamespace, "BattleDealCriticalHit", "BattleDealCriticalHit"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution Event = new()
    {
        Id = "Event",
        Name = Text.Localized(LocalizationNamespace, "Event", "Event"),
        Parameter = typeof(int),
    };

    [GameDataEntityRegistration]
    internal static readonly Evolution EventAfterDamageTaken = new()
    {
        Id = "EventAfterDamageTaken",
        Name = Text.Localized(LocalizationNamespace, "EventAfterDamageTaken", "EventAfterDamageTaken"),
        Parameter = typeof(int),
    };
}
