using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;

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
}

[GameDataRegistration<Habitat>]
[RegisterSingleton<IGameDataProvider<Habitat>>]
public partial class HabitatRegistrations
{
    private const string LocalizationNamespace = "GameData.Habitat";

    [GameDataEntityRegistration]
    internal static readonly Habitat Grassland = new()
    {
        Id = "Grassland",
        Name = Text.Localized(LocalizationNamespace, "Grassland", "Grassland"),
    };

    [GameDataEntityRegistration]
    internal static readonly Habitat Forest = new()
    {
        Id = "Forest",
        Name = Text.Localized(LocalizationNamespace, "Forest", "Forest"),
    };

    [GameDataEntityRegistration]
    internal static readonly Habitat WatersEdge = new()
    {
        Id = "WatersEdge",
        Name = Text.Localized(LocalizationNamespace, "WatersEdge", "Water's Edge"),
    };

    [GameDataEntityRegistration]
    internal static readonly Habitat Sea = new()
    {
        Id = "Sea",
        Name = Text.Localized(LocalizationNamespace, "Sea", "Sea"),
    };

    [GameDataEntityRegistration]
    internal static readonly Habitat Cave = new()
    {
        Id = "Cave",
        Name = Text.Localized(LocalizationNamespace, "Cave", "Cave"),
    };

    [GameDataEntityRegistration]
    internal static readonly Habitat Mountain = new()
    {
        Id = "Mountain",
        Name = Text.Localized(LocalizationNamespace, "Mountain", "Mountain"),
    };

    [GameDataEntityRegistration]
    internal static readonly Habitat RoughTerrain = new()
    {
        Id = "RoughTerrain",
        Name = Text.Localized(LocalizationNamespace, "RoughTerrain", "Rough Terrain"),
    };

    [GameDataEntityRegistration]
    internal static readonly Habitat Urban = new()
    {
        Id = "Urban",
        Name = Text.Localized(LocalizationNamespace, "Urban", "Urban"),
    };

    [GameDataEntityRegistration]
    internal static readonly Habitat Rare = new()
    {
        Id = "Rare",
        Name = Text.Localized(LocalizationNamespace, "Rare", "Rare"),
    };
}
