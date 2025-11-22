using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;

namespace PokeSharp.Data.Core;

/// <summary>
/// Specifies the trigger conditions for an encounter in the game.
/// </summary>
public enum EncounterTrigger : byte
{
    /// <summary>
    /// No trigger
    /// </summary>
    None,

    /// <summary>
    /// Encountered in the tall grass.
    /// </summary>
    Land,

    /// <summary>
    /// Cave encounter.
    /// </summary>
    Cave,

    /// <summary>
    /// Encountered while surfing.
    /// </summary>
    Water,

    /// <summary>
    /// Encountered while fishing.
    /// </summary>
    Fishing,

    /// <summary>
    /// Encountered during a Bug Catching Contest.
    /// </summary>
    Contest,
}

/// <summary>
/// Represents the type of an encounter within the game.
/// </summary>
/// <remarks>
/// The <c>EncounterType</c> record defines key properties associated with an encounter,
/// including its unique identifier, name, trigger type, and trigger chance.
/// It supports the addition of default values for standard encounters.
/// </remarks>
[GameDataEntity]
public partial record EncounterType
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Gets the name of the encounter type.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// Represents the trigger type of an encounter in the game.
    /// </summary>
    public required EncounterTrigger Type { get; init; }

    /// <summary>
    /// Represents the probability of triggering a specific encounter type.
    /// </summary>
    public int TriggerChance { get; init; }
}

[GameDataRegistration<EncounterType>]
[RegisterSingleton<IGameDataProvider<EncounterType>>]
public partial class EncounterTypeRegistrations
{
    private const string LocalizationNamespace = "GameData.EncounterType";

    [GameDataEntityRegistration]
    internal static readonly EncounterType Land = new()
    {
        Id = "Land",
        Name = Text.Localized(LocalizationNamespace, "Land", "Land"),
        Type = EncounterTrigger.Land,
        TriggerChance = 21,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType LandDay = new()
    {
        Id = "LandDay",
        Name = Text.Localized(LocalizationNamespace, "LandDay", "LandDay"),
        Type = EncounterTrigger.Land,
        TriggerChance = 21,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType LandNight = new()
    {
        Id = "LandNight",
        Name = Text.Localized(LocalizationNamespace, "LandNight", "LandNight"),
        Type = EncounterTrigger.Land,
        TriggerChance = 21,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType LandMorning = new()
    {
        Id = "LandMorning",
        Name = Text.Localized(LocalizationNamespace, "LandMorning", "LandMorning"),
        Type = EncounterTrigger.Land,
        TriggerChance = 21,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType LandAfternoon = new()
    {
        Id = "LandAfternoon",
        Name = Text.Localized(LocalizationNamespace, "LandAfternoon", "LandAfternoon"),
        Type = EncounterTrigger.Land,
        TriggerChance = 21,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType LandEvening = new()
    {
        Id = "LandEvening",
        Name = Text.Localized(LocalizationNamespace, "LandEvening", "LandEvening"),
        Type = EncounterTrigger.Land,
        TriggerChance = 21,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType PokeRadar = new()
    {
        Id = "PokeRadar",
        Name = Text.Localized(LocalizationNamespace, "PokeRadar", "PokeRadar"),
        Type = EncounterTrigger.Land,
        TriggerChance = 20,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType Cave = new()
    {
        Id = "Cave",
        Name = Text.Localized(LocalizationNamespace, "Cave", "Cave"),
        Type = EncounterTrigger.Cave,
        TriggerChance = 5,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType CaveDay = new()
    {
        Id = "CaveDay",
        Name = Text.Localized(LocalizationNamespace, "CaveDay", "CaveDay"),
        Type = EncounterTrigger.Cave,
        TriggerChance = 5,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType CaveNight = new()
    {
        Id = "CaveNight",
        Name = Text.Localized(LocalizationNamespace, "CaveNight", "CaveNight"),
        Type = EncounterTrigger.Cave,
        TriggerChance = 5,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType CaveMorning = new()
    {
        Id = "CaveMorning",
        Name = Text.Localized(LocalizationNamespace, "CaveMorning", "CaveMorning"),
        Type = EncounterTrigger.Cave,
        TriggerChance = 5,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType CaveAfternoon = new()
    {
        Id = "CaveAfternoon",
        Name = Text.Localized(LocalizationNamespace, "CaveAfternoon", "CaveAfternoon"),
        Type = EncounterTrigger.Cave,
        TriggerChance = 5,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType CaveEvening = new()
    {
        Id = "CaveEvening",
        Name = Text.Localized(LocalizationNamespace, "CaveEvening", "CaveEvening"),
        Type = EncounterTrigger.Cave,
        TriggerChance = 5,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType Water = new()
    {
        Id = "Water",
        Name = Text.Localized(LocalizationNamespace, "Water", "Water"),
        Type = EncounterTrigger.Water,
        TriggerChance = 2,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType WaterDay = new()
    {
        Id = "WaterDay",
        Name = Text.Localized(LocalizationNamespace, "WaterDay", "WaterDay"),
        Type = EncounterTrigger.Water,
        TriggerChance = 2,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType WaterNight = new()
    {
        Id = "WaterNight",
        Name = Text.Localized(LocalizationNamespace, "WaterNight", "WaterNight"),
        Type = EncounterTrigger.Water,
        TriggerChance = 2,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType WaterMorning = new()
    {
        Id = "WaterMorning",
        Name = Text.Localized(LocalizationNamespace, "WaterMorning", "WaterMorning"),
        Type = EncounterTrigger.Water,
        TriggerChance = 2,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType WaterAfternoon = new()
    {
        Id = "WaterAfternoon",
        Name = Text.Localized(LocalizationNamespace, "WaterAfternoon", "WaterAfternoon"),
        Type = EncounterTrigger.Water,
        TriggerChance = 2,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType WaterEvening = new()
    {
        Id = "WaterEvening",
        Name = Text.Localized(LocalizationNamespace, "WaterEvening", "WaterEvening"),
        Type = EncounterTrigger.Water,
        TriggerChance = 2,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType OldRod = new()
    {
        Id = "OldRod",
        Name = Text.Localized(LocalizationNamespace, "OldRod", "OldRod"),
        Type = EncounterTrigger.Fishing,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType GoodRod = new()
    {
        Id = "GoodRod",
        Name = Text.Localized(LocalizationNamespace, "GoodRod", "GoodRod"),
        Type = EncounterTrigger.Fishing,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType SuperRod = new()
    {
        Id = "SuperRod",
        Name = Text.Localized(LocalizationNamespace, "SuperRod", "SuperRod"),
        Type = EncounterTrigger.Fishing,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType RockSmash = new()
    {
        Id = "RockSmash",
        Name = Text.Localized(LocalizationNamespace, "RockSmash", "RockSmash"),
        Type = EncounterTrigger.None,
        TriggerChance = 50,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType HeadbuttLow = new()
    {
        Id = "HeadbuttLow",
        Name = Text.Localized(LocalizationNamespace, "HeadbuttLow", "HeadbuttLow"),
        Type = EncounterTrigger.None,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType HeadbuttHigh = new()
    {
        Id = "HeadbuttHigh",
        Name = Text.Localized(LocalizationNamespace, "HeadbuttHigh", "HeadbuttHigh"),
        Type = EncounterTrigger.None,
    };

    [GameDataEntityRegistration]
    internal static readonly EncounterType BugContest = new()
    {
        Id = "BugContest",
        Name = Text.Localized(LocalizationNamespace, "BugContest", "BugContest"),
        Type = EncounterTrigger.Contest,
        TriggerChance = 21,
    };
}
