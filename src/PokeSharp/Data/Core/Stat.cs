using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;

namespace PokeSharp.Data.Core;

/// <summary>
/// Enumeration representing the type of stat in the game.
/// </summary>
public enum StatType : byte
{
    /// <summary>
    /// Main stat, can't be raised/lowered in battle.
    /// </summary>
    Main,

    /// <summary>
    /// Main stat, can be raised/lowered in battle.
    /// </summary>
    MainBattle,

    /// <summary>
    /// Can be raised/lowered in battle, not part of the Pokémon's stats.
    /// </summary>
    Battle,
}

/// <summary>
/// Represents a statistical entity used in the context of the application.
/// </summary>
[GameDataEntity]
public partial record Stat
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Represents the name of the stat. This property is required and is of type Text.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// Represents a brief name associated with the stat.
    /// </summary>
    public required Text NameBrief { get; init; }

    /// <summary>
    /// Represents the classification of a stat, determining its context or usage, such as main stats or battle-related stats.
    /// </summary>
    public StatType StatType { get; init; }

    /// <summary>
    /// Represents the order in which the stat appears in the PBS (Pokémon Base Stats) configuration.
    /// </summary>
    public int PbsOrder { get; init; }

    /// <summary>
    /// Provides access to all <see cref="Stat"/> entities classified as either <see cref="StatType.Main"/> or <see cref="StatType.MainBattle"/>.
    /// </summary>
    /// <remarks>
    /// Intended for use cases where main or main-battle statistics need to be collated or referenced.
    /// </remarks>
    public static IEnumerable<Stat> AllMain => Entities.Where(x => x.StatType is StatType.Main or StatType.MainBattle);

    /// <summary>
    /// Retrieves a collection of all statistical entities classified specifically as <see cref="StatType.MainBattle"/>.
    /// </summary>
    public static IEnumerable<Stat> AllMainBattle => Entities.Where(x => x.StatType == StatType.MainBattle);

    /// <summary>
    /// Retrieves a collection of stats that include entities of type
    /// <see cref="StatType.Battle"/> or <see cref="StatType.MainBattle"/>.
    /// </summary>
    public static IEnumerable<Stat> AllBattle =>
        Entities.Where(x => x.StatType is StatType.Battle or StatType.MainBattle);
}

[GameDataRegistration<Stat>]
[RegisterSingleton<IGameDataProvider<Stat>>]
public partial class StatRegistrations
{
    private const string LocalizationNamespace = "GameData.Stat";

    [GameDataEntityRegistration]
    internal static readonly Stat HP = new()
    {
        Id = "HP",
        Name = Text.Localized(LocalizationNamespace, "HP", "HP"),
        NameBrief = Text.Localized(LocalizationNamespace, "HP", "HP"),
        StatType = StatType.Main,
        PbsOrder = 0,
    };

    [GameDataEntityRegistration]
    internal static readonly Stat Attack = new()
    {
        Id = "ATTACK",
        Name = Text.Localized(LocalizationNamespace, "Attack", "Attack"),
        NameBrief = Text.Localized(LocalizationNamespace, "Atk", "Atk"),
        StatType = StatType.MainBattle,
        PbsOrder = 1,
    };

    [GameDataEntityRegistration]
    internal static readonly Stat Defense = new()
    {
        Id = "DEFENSE",
        Name = Text.Localized(LocalizationNamespace, "Defense", "Defense"),
        NameBrief = Text.Localized(LocalizationNamespace, "Def", "Def"),
        StatType = StatType.MainBattle,
        PbsOrder = 2,
    };

    [GameDataEntityRegistration]
    internal static readonly Stat SpecialAttack = new()
    {
        Id = "SPECIAL_ATTACK",
        Name = Text.Localized(LocalizationNamespace, "SpecialAttack", "SpecialAttack"),
        NameBrief = Text.Localized(LocalizationNamespace, "SpAtk", "SpAtk"),
        StatType = StatType.MainBattle,
        PbsOrder = 4,
    };

    [GameDataEntityRegistration]
    internal static readonly Stat SpecialDefense = new()
    {
        Id = "SPECIAL_DEFENSE",
        Name = Text.Localized(LocalizationNamespace, "SpecialDefense", "SpecialDefense"),
        NameBrief = Text.Localized(LocalizationNamespace, "SpDef", "SpDef"),
        StatType = StatType.MainBattle,
        PbsOrder = 5,
    };

    [GameDataEntityRegistration]
    internal static readonly Stat Speed = new()
    {
        Id = "SPEED",
        Name = Text.Localized(LocalizationNamespace, "Speed", "Speed"),
        NameBrief = Text.Localized(LocalizationNamespace, "Spd", "Spd"),
        StatType = StatType.MainBattle,
        PbsOrder = 3,
    };

    [GameDataEntityRegistration]
    internal static readonly Stat Accuracy = new()
    {
        Id = "ACCURACY",
        Name = Text.Localized(LocalizationNamespace, "accuracy", "accuracy"),
        NameBrief = Text.Localized(LocalizationNamespace, "Acc", "Acc"),
        StatType = StatType.Battle,
    };

    [GameDataEntityRegistration]
    internal static readonly Stat Evasion = new()
    {
        Id = "EVASION",
        Name = Text.Localized(LocalizationNamespace, "evasiveness", "evasiveness"),
        NameBrief = Text.Localized(LocalizationNamespace, "Eva", "Eva"),
        StatType = StatType.Battle,
    };
}
