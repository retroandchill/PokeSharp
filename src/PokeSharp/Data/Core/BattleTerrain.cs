using Injectio.Attributes;
using PokeSharp.Abstractions;
using PokeSharp.Core.Data;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents a specific battle terrain within the game, identified by a unique <see cref="Name"/>.
/// </summary>
/// <remarks>
/// Battle terrain influences the environment of a Pokémon battle, often altering mechanics or providing specific benefits/effects based on the terrain type.
/// </remarks>
[GameDataEntity]
public readonly partial record struct BattleTerrain
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Gets or sets the localized name of the battle terrain.
    /// </summary>
    /// <remarks>
    /// The <see cref="Name"/> property represents a descriptive and localized text identifier
    /// for the terrain, which may be displayed to the player or used in-game to differentiate terrains.
    /// </remarks>
    public required Text Name { get; init; }
}

[GameDataRegistration<BattleTerrain>]
[RegisterSingleton<IGameDataProvider<BattleTerrain>>]
public partial class BattleTerrainRegistrations
{
    private const string LocalizationNamespace = "GameData.BattleTerrain";

    [GameDataEntityRegistration]
    internal static readonly BattleTerrain Electric = new()
    {
        Id = "Electric",
        Name = Text.Localized(LocalizationNamespace, "Electric", "Electric"),
    };

    [GameDataEntityRegistration]
    internal static readonly BattleTerrain Grassy = new()
    {
        Id = "Grassy",
        Name = Text.Localized(LocalizationNamespace, "Grassy", "Grassy"),
    };

    [GameDataEntityRegistration]
    internal static readonly BattleTerrain Misty = new()
    {
        Id = "Misty",
        Name = Text.Localized(LocalizationNamespace, "Misty", "Misty"),
    };

    [GameDataEntityRegistration]
    internal static readonly BattleTerrain Psychic = new()
    {
        Id = "Psychic",
        Name = Text.Localized(LocalizationNamespace, "Psychic", "Psychic"),
    };
}
