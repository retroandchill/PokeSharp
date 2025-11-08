using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

[GameDataEntity]
public readonly partial record struct Status
{

    public required Name Id { get; init; }

    public required Text Name { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.Status";

    public static void AddDefaultValues()
    {
        Register(
            new Status
            {
                Id = "SLEEP",
                Name = Text.Localized(LocalizationNamespace, "SLEEP", "Sleep"),
            }
        );

        Register(
            new Status
            {
                Id = "POISON",
                Name = Text.Localized(LocalizationNamespace, "POISON", "Poison"),
            }
        );

        Register(
            new Status
            {
                Id = "BURN",
                Name = Text.Localized(LocalizationNamespace, "BURN", "Burn"),
            }
        );

        Register(
            new Status
            {
                Id = "PARALYSIS",
                Name = Text.Localized(LocalizationNamespace, "PARALYSIS", "Paralysis"),
            }
        );

        Register(
            new Status
            {
                Id = "FROZEN",
                Name = Text.Localized(LocalizationNamespace, "FROZEN", "Frozen"),
            }
        );

    }
    #endregion
}