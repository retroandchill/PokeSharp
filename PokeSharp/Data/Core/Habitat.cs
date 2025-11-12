using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents a habitat for Pokémon, categorized by different environmental or geographical features.
/// </summary>
/// <remarks>
/// This type is used to classify Pokémon habitats into predefined categories such as Grassland,
/// Forest, WatersEdge, or similar. Habitat information is used within the Pokémon data model
/// to associate species with their natural or typical environments.
/// </remarks>
[GameDataEntity]
public readonly partial record struct Habitat
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Represents the localized name of the habitat, allowing for translations and multi-language support.
    /// </summary>
    public required Text Name { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.Habitat";

    /// <summary>
    /// Populates the Habitat entity with a predefined set of default values.
    /// </summary>
    /// <remarks>
    /// This method registers a collection of default habitats, each represented by a unique identifier and corresponding name.
    /// It is utilized to ensure that the Habitat data includes commonly used predefined entries such as Grassland, Forest, Sea, and others.
    /// </remarks>
    public static void AddDefaultValues()
    {
        Register(
            new Habitat { Id = "Grassland", Name = Text.Localized(LocalizationNamespace, "Grassland", "Grassland") }
        );

        Register(new Habitat { Id = "Forest", Name = Text.Localized(LocalizationNamespace, "Forest", "Forest") });

        Register(
            new Habitat
            {
                Id = "WatersEdge",
                Name = Text.Localized(LocalizationNamespace, "WatersEdge", "Water's Edge"),
            }
        );

        Register(new Habitat { Id = "Sea", Name = Text.Localized(LocalizationNamespace, "Sea", "Sea") });

        Register(new Habitat { Id = "Cave", Name = Text.Localized(LocalizationNamespace, "Cave", "Cave") });

        Register(new Habitat { Id = "Mountain", Name = Text.Localized(LocalizationNamespace, "Mountain", "Mountain") });

        Register(
            new Habitat
            {
                Id = "RoughTerrain",
                Name = Text.Localized(LocalizationNamespace, "RoughTerrain", "Rough Terrain"),
            }
        );

        Register(new Habitat { Id = "Urban", Name = Text.Localized(LocalizationNamespace, "Urban", "Urban") });

        Register(new Habitat { Id = "Rare", Name = Text.Localized(LocalizationNamespace, "Rare", "Rare") });
    }
    #endregion
}
