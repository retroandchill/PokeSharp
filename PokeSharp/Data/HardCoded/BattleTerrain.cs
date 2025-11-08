using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.HardCoded;

[GameDataEntity]
public readonly partial record struct BattleTerrain
{

    public required Name Id { get; init; }

    public required Text Name { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.BattleTerrain";

    public static void AddDefaultValues()
    {
        Register(
            new BattleTerrain
            {
                Id = "Electric",
                Name = Text.Localized(LocalizationNamespace, "Electric", "Electric"),
            }
        );

        Register(
            new BattleTerrain
            {
                Id = "Grassy",
                Name = Text.Localized(LocalizationNamespace, "Grassy", "Grassy"),
            }
        );

        Register(
            new BattleTerrain
            {
                Id = "Misty",
                Name = Text.Localized(LocalizationNamespace, "Misty", "Misty"),
            }
        );

        Register(
            new BattleTerrain
            {
                Id = "Psychic",
                Name = Text.Localized(LocalizationNamespace, "Psychic", "Psychic"),
            }
        );

    }
    #endregion
}