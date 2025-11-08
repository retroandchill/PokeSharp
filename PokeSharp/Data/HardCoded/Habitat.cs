using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.HardCoded;

[GameDataEntity]
public readonly partial record struct Habitat
{

    public required Name Id { get; init; }

    public required Text Name { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.Habitat";

    public static void AddDefaultValues()
    {
        Register(
            new Habitat
            {
                Id = "Grassland",
                Name = Text.Localized(LocalizationNamespace, "Grassland", "Grassland"),
            }
        );

        Register(
            new Habitat
            {
                Id = "Forest",
                Name = Text.Localized(LocalizationNamespace, "Forest", "Forest"),
            }
        );

        Register(
            new Habitat
            {
                Id = "WatersEdge",
                Name = Text.Localized(LocalizationNamespace, "WatersEdge", "Water's Edge"),
            }
        );

        Register(
            new Habitat
            {
                Id = "Sea",
                Name = Text.Localized(LocalizationNamespace, "Sea", "Sea"),
            }
        );

        Register(
            new Habitat
            {
                Id = "Cave",
                Name = Text.Localized(LocalizationNamespace, "Cave", "Cave"),
            }
        );

        Register(
            new Habitat
            {
                Id = "Mountain",
                Name = Text.Localized(LocalizationNamespace, "Mountain", "Mountain"),
            }
        );

        Register(
            new Habitat
            {
                Id = "RoughTerrain",
                Name = Text.Localized(LocalizationNamespace, "RoughTerrain", "Rough Terrain"),
            }
        );

        Register(
            new Habitat
            {
                Id = "Urban",
                Name = Text.Localized(LocalizationNamespace, "Urban", "Urban"),
            }
        );

        Register(
            new Habitat
            {
                Id = "Rare",
                Name = Text.Localized(LocalizationNamespace, "Rare", "Rare"),
            }
        );

    }
    #endregion
}
