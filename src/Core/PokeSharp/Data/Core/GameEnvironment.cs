using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents a game environment entity in the system, which includes an identifier,
/// a display name, and an optional base environment used for battles.
/// </summary>
[GameDataEntity]
public partial record GameEnvironment
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Gets or initializes the name associated with the game environment.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// Represents the base name reference for a battle environment in the game.
    /// Used within the GameEnvironment structure to provide a unique or identifiable
    /// label for battle-related contexts.
    /// </summary>
    public Name BattleBase { get; init; }
}

[GameDataRegistration<GameEnvironment>]
[RegisterSingleton<IGameDataProvider<GameEnvironment>>]
public partial class GameEnvironmentRegistrations
{
    private const string LocalizationNamespace = "GameData.GameEnvironment";

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Grass = new()
    {
        Id = "Grass",
        Name = Text.Localized(LocalizationNamespace, "Grass", "Grass"),
        BattleBase = "grass",
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment TallGrass = new()
    {
        Id = "TallGrass",
        Name = Text.Localized(LocalizationNamespace, "TallGrass", "Tall grass"),
        BattleBase = "grass",
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment MovingWater = new()
    {
        Id = "MovingWater",
        Name = Text.Localized(LocalizationNamespace, "MovingWater", "Moving water"),
        BattleBase = "water",
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment StillWater = new()
    {
        Id = "StillWater",
        Name = Text.Localized(LocalizationNamespace, "StillWater", "Still water"),
        BattleBase = "water",
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Puddle = new()
    {
        Id = "Puddle",
        Name = Text.Localized(LocalizationNamespace, "Puddle", "Puddle"),
        BattleBase = "puddle",
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Underwater = new()
    {
        Id = "Underwater",
        Name = Text.Localized(LocalizationNamespace, "Underwater", "Underwater"),
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Cave = new()
    {
        Id = "Cave",
        Name = Text.Localized(LocalizationNamespace, "Cave", "Cave"),
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Rock = new()
    {
        Id = "Rock",
        Name = Text.Localized(LocalizationNamespace, "Rock", "Rock"),
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Sand = new()
    {
        Id = "Sand",
        Name = Text.Localized(LocalizationNamespace, "Sand", "Sand"),
        BattleBase = "sand",
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Forest = new()
    {
        Id = "Forest",
        Name = Text.Localized(LocalizationNamespace, "Forest", "Forest"),
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment ForestGrass = new()
    {
        Id = "ForestGrass",
        Name = Text.Localized(LocalizationNamespace, "ForestGrass", "Forest grass"),
        BattleBase = "grass",
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Snow = new()
    {
        Id = "Snow",
        Name = Text.Localized(LocalizationNamespace, "Snow", "Snow"),
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Ice = new()
    {
        Id = "Ice",
        Name = Text.Localized(LocalizationNamespace, "Ice", "Ice"),
        BattleBase = "ice",
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Volcano = new()
    {
        Id = "Volcano",
        Name = Text.Localized(LocalizationNamespace, "Volcano", "Volcano"),
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Graveyard = new()
    {
        Id = "Graveyard",
        Name = Text.Localized(LocalizationNamespace, "Graveyard", "Graveyard"),
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Sky = new()
    {
        Id = "Sky",
        Name = Text.Localized(LocalizationNamespace, "Sky", "Sky"),
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment Space = new()
    {
        Id = "Space",
        Name = Text.Localized(LocalizationNamespace, "Space", "Space"),
    };

    [GameDataEntityRegistration]
    internal static readonly GameEnvironment UltraSpace = new()
    {
        Id = "UltraSpace",
        Name = Text.Localized(LocalizationNamespace, "UltraSpace", "Ultra Space"),
    };
}
