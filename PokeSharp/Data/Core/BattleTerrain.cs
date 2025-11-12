using PokeSharp.Abstractions;
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

    #region Defaults

    private const string LocalizationNamespace = "GameData.BattleTerrain";

    /// <summary>
    /// Populates the BattleTerrain entity with default terrain values by registering a set of predefined terrains.
    /// </summary>
    /// <remarks>
    /// This method adds the default terrain entries such as Electric, Grassy, Misty, and Psychic,
    /// each initialized with a specific ID and localized name. The data is intended to provide
    /// baseline configurations for terrains in the game environment.
    /// </remarks>
    public static void AddDefaultValues()
    {
        Register(
            new BattleTerrain { Id = "Electric", Name = Text.Localized(LocalizationNamespace, "Electric", "Electric") }
        );

        Register(new BattleTerrain { Id = "Grassy", Name = Text.Localized(LocalizationNamespace, "Grassy", "Grassy") });

        Register(new BattleTerrain { Id = "Misty", Name = Text.Localized(LocalizationNamespace, "Misty", "Misty") });

        Register(
            new BattleTerrain { Id = "Psychic", Name = Text.Localized(LocalizationNamespace, "Psychic", "Psychic") }
        );
    }
    #endregion
}
