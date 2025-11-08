using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.HardCoded;

[GameDataEntity]
public readonly partial record struct BodyColor
{

    public required Name Id { get; init; }
    
    public required Text Name { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.BodyColor";

    public static void AddDefaultValues()
    {

        Register(
            new BodyColor
            {
                Id = "Red",
                Name = Text.Localized(LocalizationNamespace, "Red", "Red"),
            }
        );

        Register(
            new BodyColor
            {
                Id = "Blue",
                Name = Text.Localized(LocalizationNamespace, "Blue", "Blue"),
            }
        );

        Register(
            new BodyColor
            {
                Id = "Yellow",
                Name = Text.Localized(LocalizationNamespace, "Yellow", "Yellow"),
            }
        );

        Register(
            new BodyColor
            {
                Id = "Green",
                Name = Text.Localized(LocalizationNamespace, "Green", "Green"),
            }
        );

        Register(
            new BodyColor
            {
                Id = "Black",
                Name = Text.Localized(LocalizationNamespace, "Black", "Black"),
            }
        );

        Register(
            new BodyColor
            {
                Id = "Brown",
                Name = Text.Localized(LocalizationNamespace, "Brown", "Brown"),
            }
        );

        Register(
            new BodyColor
            {
                Id = "Purple",
                Name = Text.Localized(LocalizationNamespace, "Purple", "Purple"),
            }
        );

        Register(
            new BodyColor
            {
                Id = "Gray",
                Name = Text.Localized(LocalizationNamespace, "Gray", "Gray"),
            }
        );

        Register(
            new BodyColor
            {
                Id = "White",
                Name = Text.Localized(LocalizationNamespace, "White", "White"),
            }
        );

        Register(
            new BodyColor
            {
                Id = "Pink",
                Name = Text.Localized(LocalizationNamespace, "Pink", "Pink"),
            }
        );

    }
    #endregion
}
