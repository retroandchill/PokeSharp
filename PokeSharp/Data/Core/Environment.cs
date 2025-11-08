using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

[GameDataEntity]
public partial record Environment
{

    public required Name Id { get; init; }

    public required Text Name { get; init; }
    
    public Name BattleBase { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.Environment";

    public static void AddDefaultValues()
    {
        Register(
            new Environment
            {
                Id = "Grass",
                Name = Text.Localized(LocalizationNamespace, "Grass", "Grass"),
                BattleBase = "grass"
            }
        );

        Register(
            new Environment
            {
                Id = "TallGrass",
                Name = Text.Localized(LocalizationNamespace, "TallGrass", "Tall grass"),
                BattleBase = "grass"
            }
        );

        Register(
            new Environment
            {
                Id = "MovingWater",
                Name = Text.Localized(LocalizationNamespace, "MovingWater", "Moving water"),
                BattleBase = "water"
            }
        );

        Register(
            new Environment
            {
                Id = "StillWater",
                Name = Text.Localized(LocalizationNamespace, "StillWater", "Still water"),
                BattleBase = "water"
            }
        );

        Register(
            new Environment
            {
                Id = "Puddle",
                Name = Text.Localized(LocalizationNamespace, "Puddle", "Puddle"),
                BattleBase = "puddle"
            }
        );

        Register(
            new Environment
            {
                Id = "Underwater",
                Name = Text.Localized(LocalizationNamespace, "Underwater", "Underwater")
            }
        );

        Register(
            new Environment
            {
                Id = "Cave",
                Name = Text.Localized(LocalizationNamespace, "Cave", "Cave")
            }
        );

        Register(
            new Environment
            {
                Id = "Rock",
                Name = Text.Localized(LocalizationNamespace, "Rock", "Rock")
            }
        );

        Register(
            new Environment
            {
                Id = "Sand",
                Name = Text.Localized(LocalizationNamespace, "Sand", "Sand"),
                BattleBase = "sand",
            }
        );

        Register(
            new Environment
            {
                Id = "Forest",
                Name = Text.Localized(LocalizationNamespace, "Forest", "Forest")
            }
        );

        Register(
            new Environment
            {
                Id = "ForestGrass",
                Name = Text.Localized(LocalizationNamespace, "ForestGrass", "Forest grass"),
                BattleBase = "grass",
            }
        );

        Register(
            new Environment
            {
                Id = "Snow",
                Name = Text.Localized(LocalizationNamespace, "Snow", "Snow")
            }
        );

        Register(
            new Environment
            {
                Id = "Ice",
                Name = Text.Localized(LocalizationNamespace, "Ice", "Ice"),
                BattleBase = "ice",
            }
        );

        Register(
            new Environment
            {
                Id = "Volcano",
                Name = Text.Localized(LocalizationNamespace, "Volcano", "Volcano")
            }
        );

        Register(
            new Environment
            {
                Id = "Graveyard",
                Name = Text.Localized(LocalizationNamespace, "Graveyard", "Graveyard")
            }
        );

        Register(
            new Environment
            {
                Id = "Sky",
                Name = Text.Localized(LocalizationNamespace, "Sky", "Sky")
            }
        );

        Register(
            new Environment
            {
                Id = "Space",
                Name = Text.Localized(LocalizationNamespace, "Space", "Space")
            }
        );

        Register(
            new Environment
            {
                Id = "UltraSpace",
                Name = Text.Localized(LocalizationNamespace, "UltraSpace", "Ultra Space")
            }
        );

    }
    #endregion
}
