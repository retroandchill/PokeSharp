using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents a change in a specific stat of a Pokémon.
/// </summary>
/// <param name="Stat">The stat that is being changed.</param>
/// <param name="Change">The amount of change in the stat.</param>
public readonly record struct StatChange(Name Stat, int Change);

/// <summary>
/// Represents a nature in the game, which affects the stat changes for a Pokémon.
/// </summary>
[GameDataEntity]
public partial record Nature
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Represents the name of the nature as a required text value.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// Represents the collection of stat changes associated with a specific nature.
    /// </summary>
    public ImmutableArray<StatChange> StatChanges { get; init; } = [];
}

[GameDataRegistration<Nature>]
[RegisterSingleton<IGameDataProvider<Nature>>]
public partial class NatureRegistrations
{
    private const string LocalizationNamespace = "GameData.Nature";

    [GameDataEntityRegistration]
    internal static readonly Nature Hardy = new()
    {
        Id = "HARDY",
        Name = Text.Localized(LocalizationNamespace, "HARDY", "Hardy"),
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Lonely = new()
    {
        Id = "LONELY",
        Name = Text.Localized(LocalizationNamespace, "LONELY", "Lonely"),
        StatChanges = [new StatChange(Stat.Attack.Id, 10), new StatChange(Stat.Defense.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Brave = new()
    {
        Id = "BRAVE",
        Name = Text.Localized(LocalizationNamespace, "BRAVE", "Brave"),
        StatChanges = [new StatChange(Stat.Attack.Id, 10), new StatChange(Stat.Speed.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Adamant = new()
    {
        Id = "ADAMANT",
        Name = Text.Localized(LocalizationNamespace, "ADAMANT", "Adamant"),
        StatChanges = [new StatChange(Stat.Attack.Id, 10), new StatChange(Stat.SpecialAttack.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Naughty = new()
    {
        Id = "NAUGHTY",
        Name = Text.Localized(LocalizationNamespace, "NAUGHTY", "Naughty"),
        StatChanges = [new StatChange(Stat.Attack.Id, 10), new StatChange(Stat.SpecialDefense.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Bold = new()
    {
        Id = "BOLD",
        Name = Text.Localized(LocalizationNamespace, "BOLD", "Bold"),
        StatChanges = [new StatChange(Stat.Defense.Id, 10), new StatChange(Stat.Attack.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Docile = new()
    {
        Id = "DOCILE",
        Name = Text.Localized(LocalizationNamespace, "DOCILE", "Docile"),
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Relaxed = new()
    {
        Id = "RELAXED",
        Name = Text.Localized(LocalizationNamespace, "RELAXED", "Relaxed"),
        StatChanges = [new StatChange(Stat.Defense.Id, 10), new StatChange(Stat.Speed.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Impish = new()
    {
        Id = "IMPISH",
        Name = Text.Localized(LocalizationNamespace, "IMPISH", "Impish"),
        StatChanges = [new StatChange(Stat.Defense.Id, 10), new StatChange(Stat.SpecialAttack.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Lax = new()
    {
        Id = "LAX",
        Name = Text.Localized(LocalizationNamespace, "LAX", "Lax"),
        StatChanges = [new StatChange(Stat.Defense.Id, 10), new StatChange(Stat.SpecialDefense.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Timid = new()
    {
        Id = "TIMID",
        Name = Text.Localized(LocalizationNamespace, "TIMID", "Timid"),
        StatChanges = [new StatChange(Stat.Speed.Id, 10), new StatChange(Stat.Attack.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Hasty = new()
    {
        Id = "HASTY",
        Name = Text.Localized(LocalizationNamespace, "HASTY", "Hasty"),
        StatChanges = [new StatChange(Stat.Speed.Id, 10), new StatChange(Stat.Defense.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Serious = new()
    {
        Id = "SERIOUS",
        Name = Text.Localized(LocalizationNamespace, "SERIOUS", "Serious"),
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Jolly = new()
    {
        Id = "JOLLY",
        Name = Text.Localized(LocalizationNamespace, "JOLLY", "Jolly"),
        StatChanges = [new StatChange(Stat.Speed.Id, 10), new StatChange(Stat.SpecialAttack.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Naive = new()
    {
        Id = "NAIVE",
        Name = Text.Localized(LocalizationNamespace, "NAIVE", "Naive"),
        StatChanges = [new StatChange(Stat.Speed.Id, 10), new StatChange(Stat.SpecialDefense.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Modest = new()
    {
        Id = "MODEST",
        Name = Text.Localized(LocalizationNamespace, "MODEST", "Modest"),
        StatChanges = [new StatChange(Stat.SpecialAttack.Id, 10), new StatChange(Stat.Attack.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Mild = new()
    {
        Id = "MILD",
        Name = Text.Localized(LocalizationNamespace, "MILD", "Mild"),
        StatChanges = [new StatChange(Stat.SpecialAttack.Id, 10), new StatChange(Stat.Defense.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Quiet = new()
    {
        Id = "QUIET",
        Name = Text.Localized(LocalizationNamespace, "QUIET", "Quiet"),
        StatChanges = [new StatChange(Stat.SpecialAttack.Id, 10), new StatChange(Stat.Speed.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Bashful = new()
    {
        Id = "BASHFUL",
        Name = Text.Localized(LocalizationNamespace, "BASHFUL", "Bashful"),
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Rash = new()
    {
        Id = "RASH",
        Name = Text.Localized(LocalizationNamespace, "RASH", "Rash"),
        StatChanges = [new StatChange(Stat.SpecialAttack.Id, 10), new StatChange(Stat.SpecialDefense.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Calm = new()
    {
        Id = "CALM",
        Name = Text.Localized(LocalizationNamespace, "CALM", "Calm"),
        StatChanges = [new StatChange(Stat.SpecialDefense.Id, 10), new StatChange(Stat.Attack.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Gentle = new()
    {
        Id = "GENTLE",
        Name = Text.Localized(LocalizationNamespace, "GENTLE", "Gentle"),
        StatChanges = [new StatChange(Stat.SpecialDefense.Id, 10), new StatChange(Stat.Defense.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Sassy = new()
    {
        Id = "SASSY",
        Name = Text.Localized(LocalizationNamespace, "SASSY", "Sassy"),
        StatChanges = [new StatChange(Stat.SpecialDefense.Id, 10), new StatChange(Stat.Speed.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Careful = new()
    {
        Id = "CAREFUL",
        Name = Text.Localized(LocalizationNamespace, "CAREFUL", "Careful"),
        StatChanges = [new StatChange(Stat.SpecialDefense.Id, 10), new StatChange(Stat.SpecialAttack.Id, -10)],
    };

    [GameDataEntityRegistration]
    internal static readonly Nature Quirky = new()
    {
        Id = "QUIRKY",
        Name = Text.Localized(LocalizationNamespace, "QUIRKY", "Quirky"),
    };
}
