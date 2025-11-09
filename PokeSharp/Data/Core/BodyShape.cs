using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

[GameDataEntity]
public readonly partial record struct BodyShape
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.BodyShape";

    public static void AddDefaultValues()
    {
        Register(
            new BodyShape
            {
                Id = "Head",
                Name = Text.Localized(LocalizationNamespace, "Head", "Head"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "Serpentine",
                Name = Text.Localized(LocalizationNamespace, "Serpentine", "Serpentine"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "Finned",
                Name = Text.Localized(LocalizationNamespace, "Finned", "Finned"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "HeadArms",
                Name = Text.Localized(LocalizationNamespace, "HeadArms", "Head and arms"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "HeadBase",
                Name = Text.Localized(LocalizationNamespace, "HeadBase", "Head and base"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "BipedalTail",
                Name = Text.Localized(LocalizationNamespace, "BipedalTail", "Bipedal with tail"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "HeadLegs",
                Name = Text.Localized(LocalizationNamespace, "HeadLegs", "Head and legs"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "Quadruped",
                Name = Text.Localized(LocalizationNamespace, "Quadruped", "Quadruped"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "Winged",
                Name = Text.Localized(LocalizationNamespace, "Winged", "Winged"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "Multiped",
                Name = Text.Localized(LocalizationNamespace, "Multiped", "Multiped"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "MultiBody",
                Name = Text.Localized(LocalizationNamespace, "MultiBody", "Multi Body"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "Bipedal",
                Name = Text.Localized(LocalizationNamespace, "Bipedal", "Bipedal"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "MultiWinged",
                Name = Text.Localized(LocalizationNamespace, "MultiWinged", "Multi Winged"),
            }
        );

        Register(
            new BodyShape
            {
                Id = "Insectoid",
                Name = Text.Localized(LocalizationNamespace, "Insectoid", "Insectoid"),
            }
        );
    }
    #endregion
}
