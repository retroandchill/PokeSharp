using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

[GameDataEntity]
public partial record GameEnvironment
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    public Name BattleBase { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.Environment";

    public static void AddDefaultValues()
    {
        Register(
            new GameEnvironment
            {
                Id = "Grass",
                Name = Text.Localized(LocalizationNamespace, "Grass", "Grass"),
                BattleBase = "grass",
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "TallGrass",
                Name = Text.Localized(LocalizationNamespace, "TallGrass", "Tall grass"),
                BattleBase = "grass",
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "MovingWater",
                Name = Text.Localized(LocalizationNamespace, "MovingWater", "Moving water"),
                BattleBase = "water",
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "StillWater",
                Name = Text.Localized(LocalizationNamespace, "StillWater", "Still water"),
                BattleBase = "water",
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Puddle",
                Name = Text.Localized(LocalizationNamespace, "Puddle", "Puddle"),
                BattleBase = "puddle",
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Underwater",
                Name = Text.Localized(LocalizationNamespace, "Underwater", "Underwater"),
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Cave",
                Name = Text.Localized(LocalizationNamespace, "Cave", "Cave"),
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Rock",
                Name = Text.Localized(LocalizationNamespace, "Rock", "Rock"),
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Sand",
                Name = Text.Localized(LocalizationNamespace, "Sand", "Sand"),
                BattleBase = "sand",
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Forest",
                Name = Text.Localized(LocalizationNamespace, "Forest", "Forest"),
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "ForestGrass",
                Name = Text.Localized(LocalizationNamespace, "ForestGrass", "Forest grass"),
                BattleBase = "grass",
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Snow",
                Name = Text.Localized(LocalizationNamespace, "Snow", "Snow"),
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Ice",
                Name = Text.Localized(LocalizationNamespace, "Ice", "Ice"),
                BattleBase = "ice",
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Volcano",
                Name = Text.Localized(LocalizationNamespace, "Volcano", "Volcano"),
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Graveyard",
                Name = Text.Localized(LocalizationNamespace, "Graveyard", "Graveyard"),
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Sky",
                Name = Text.Localized(LocalizationNamespace, "Sky", "Sky"),
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "Space",
                Name = Text.Localized(LocalizationNamespace, "Space", "Space"),
            }
        );

        Register(
            new GameEnvironment
            {
                Id = "UltraSpace",
                Name = Text.Localized(LocalizationNamespace, "UltraSpace", "Ultra Space"),
            }
        );
    }
    #endregion
}
