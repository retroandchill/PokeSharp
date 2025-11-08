using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

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

[GameDataEntity]
public partial record EncounterType
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    public required EncounterTrigger Type { get; init; }

    public int TriggerChance { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.EncounterType";

    public static void AddDefaultValues()
    {
        Register(
            new EncounterType
            {
                Id = "Land",
                Name = Text.Localized(LocalizationNamespace, "Land", "Land"),
                Type = EncounterTrigger.Land,
                TriggerChance = 21,
            }
        );

        Register(
            new EncounterType
            {
                Id = "LandDay",
                Name = Text.Localized(LocalizationNamespace, "LandDay", "LandDay"),
                Type = EncounterTrigger.Land,
                TriggerChance = 21,
            }
        );

        Register(
            new EncounterType
            {
                Id = "LandNight",
                Name = Text.Localized(LocalizationNamespace, "LandNight", "LandNight"),
                Type = EncounterTrigger.Land,
                TriggerChance = 21,
            }
        );

        Register(
            new EncounterType
            {
                Id = "LandMorning",
                Name = Text.Localized(LocalizationNamespace, "LandMorning", "LandMorning"),
                Type = EncounterTrigger.Land,
                TriggerChance = 21,
            }
        );

        Register(
            new EncounterType
            {
                Id = "LandAfternoon",
                Name = Text.Localized(LocalizationNamespace, "LandAfternoon", "LandAfternoon"),
                Type = EncounterTrigger.Land,
                TriggerChance = 21,
            }
        );

        Register(
            new EncounterType
            {
                Id = "LandEvening",
                Name = Text.Localized(LocalizationNamespace, "LandEvening", "LandEvening"),
                Type = EncounterTrigger.Land,
                TriggerChance = 21,
            }
        );

        Register(
            new EncounterType
            {
                Id = "PokeRadar",
                Name = Text.Localized(LocalizationNamespace, "PokeRadar", "PokeRadar"),
                Type = EncounterTrigger.Land,
                TriggerChance = 20,
            }
        );

        Register(
            new EncounterType
            {
                Id = "Cave",
                Name = Text.Localized(LocalizationNamespace, "Cave", "Cave"),
                Type = EncounterTrigger.Cave,
                TriggerChance = 5,
            }
        );

        Register(
            new EncounterType
            {
                Id = "CaveDay",
                Name = Text.Localized(LocalizationNamespace, "CaveDay", "CaveDay"),
                Type = EncounterTrigger.Cave,
                TriggerChance = 5,
            }
        );

        Register(
            new EncounterType
            {
                Id = "CaveNight",
                Name = Text.Localized(LocalizationNamespace, "CaveNight", "CaveNight"),
                Type = EncounterTrigger.Cave,
                TriggerChance = 5,
            }
        );

        Register(
            new EncounterType
            {
                Id = "CaveMorning",
                Name = Text.Localized(LocalizationNamespace, "CaveMorning", "CaveMorning"),
                Type = EncounterTrigger.Cave,
                TriggerChance = 5,
            }
        );

        Register(
            new EncounterType
            {
                Id = "CaveAfternoon",
                Name = Text.Localized(LocalizationNamespace, "CaveAfternoon", "CaveAfternoon"),
                Type = EncounterTrigger.Cave,
                TriggerChance = 5,
            }
        );

        Register(
            new EncounterType
            {
                Id = "CaveEvening",
                Name = Text.Localized(LocalizationNamespace, "CaveEvening", "CaveEvening"),
                Type = EncounterTrigger.Cave,
                TriggerChance = 5,
            }
        );

        Register(
            new EncounterType
            {
                Id = "Water",
                Name = Text.Localized(LocalizationNamespace, "Water", "Water"),
                Type = EncounterTrigger.Water,
                TriggerChance = 2,
            }
        );

        Register(
            new EncounterType
            {
                Id = "WaterDay",
                Name = Text.Localized(LocalizationNamespace, "WaterDay", "WaterDay"),
                Type = EncounterTrigger.Water,
                TriggerChance = 2,
            }
        );

        Register(
            new EncounterType
            {
                Id = "WaterNight",
                Name = Text.Localized(LocalizationNamespace, "WaterNight", "WaterNight"),
                Type = EncounterTrigger.Water,
                TriggerChance = 2,
            }
        );

        Register(
            new EncounterType
            {
                Id = "WaterMorning",
                Name = Text.Localized(LocalizationNamespace, "WaterMorning", "WaterMorning"),
                Type = EncounterTrigger.Water,
                TriggerChance = 2,
            }
        );

        Register(
            new EncounterType
            {
                Id = "WaterAfternoon",
                Name = Text.Localized(LocalizationNamespace, "WaterAfternoon", "WaterAfternoon"),
                Type = EncounterTrigger.Water,
                TriggerChance = 2,
            }
        );

        Register(
            new EncounterType
            {
                Id = "WaterEvening",
                Name = Text.Localized(LocalizationNamespace, "WaterEvening", "WaterEvening"),
                Type = EncounterTrigger.Water,
                TriggerChance = 2,
            }
        );

        Register(
            new EncounterType
            {
                Id = "OldRod",
                Name = Text.Localized(LocalizationNamespace, "OldRod", "OldRod"),
                Type = EncounterTrigger.Fishing,
            }
        );

        Register(
            new EncounterType
            {
                Id = "GoodRod",
                Name = Text.Localized(LocalizationNamespace, "GoodRod", "GoodRod"),
                Type = EncounterTrigger.Fishing,
            }
        );

        Register(
            new EncounterType
            {
                Id = "SuperRod",
                Name = Text.Localized(LocalizationNamespace, "SuperRod", "SuperRod"),
                Type = EncounterTrigger.Fishing,
            }
        );

        Register(
            new EncounterType
            {
                Id = "RockSmash",
                Name = Text.Localized(LocalizationNamespace, "RockSmash", "RockSmash"),
                Type = EncounterTrigger.None,
                TriggerChance = 50,
            }
        );

        Register(
            new EncounterType
            {
                Id = "HeadbuttLow",
                Name = Text.Localized(LocalizationNamespace, "HeadbuttLow", "HeadbuttLow"),
                Type = EncounterTrigger.None,
            }
        );

        Register(
            new EncounterType
            {
                Id = "HeadbuttHigh",
                Name = Text.Localized(LocalizationNamespace, "HeadbuttHigh", "HeadbuttHigh"),
                Type = EncounterTrigger.None,
            }
        );

        Register(
            new EncounterType
            {
                Id = "BugContest",
                Name = Text.Localized(LocalizationNamespace, "BugContest", "BugContest"),
                Type = EncounterTrigger.Contest,
                TriggerChance = 21,
            }
        );
    }

    #endregion
}