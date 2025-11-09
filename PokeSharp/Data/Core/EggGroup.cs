using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

public enum EggGroupType : byte
{
    /// <summary>
    /// Requires the other Pokémon to have the same egg group.
    /// </summary>
    WithSameEggGroup,

    /// <summary>
    /// Can breed with any egg group.
    /// </summary>
    WithAnyEggGroup,

    /// <summary>
    /// Cannot breed with any egg group.
    /// </summary>
    WithNoEggGroups,
}

[GameDataEntity]
public readonly partial record struct EggGroup
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    public EggGroupType BreedingType { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.EggGroup";

    public static void AddDefaultValues()
    {
        Register(
            new EggGroup
            {
                Id = "Undiscovered",
                Name = Text.Localized(LocalizationNamespace, "Undiscovered", "Undiscovered"),
            }
        );

        Register(
            new EggGroup
            {
                Id = "Monster",
                Name = Text.Localized(LocalizationNamespace, "Monster", "Monster"),
            }
        );

        Register(
            new EggGroup
            {
                Id = "Water1",
                Name = Text.Localized(LocalizationNamespace, "Water1", "Water 1"),
            }
        );

        Register(
            new EggGroup { Id = "Bug", Name = Text.Localized(LocalizationNamespace, "Bug", "Bug") }
        );

        Register(
            new EggGroup
            {
                Id = "Flying",
                Name = Text.Localized(LocalizationNamespace, "Flying", "Flying"),
            }
        );

        Register(
            new EggGroup
            {
                Id = "Field",
                Name = Text.Localized(LocalizationNamespace, "Field", "Field"),
            }
        );

        Register(
            new EggGroup
            {
                Id = "Fairy",
                Name = Text.Localized(LocalizationNamespace, "Fairy", "Fairy"),
            }
        );

        Register(
            new EggGroup
            {
                Id = "Grass",
                Name = Text.Localized(LocalizationNamespace, "Grass", "Grass"),
            }
        );

        Register(
            new EggGroup
            {
                Id = "Humanlike",
                Name = Text.Localized(LocalizationNamespace, "Humanlike", "Humanlike"),
            }
        );

        Register(
            new EggGroup
            {
                Id = "Water3",
                Name = Text.Localized(LocalizationNamespace, "Water3", "Water 3"),
            }
        );

        Register(
            new EggGroup
            {
                Id = "Mineral",
                Name = Text.Localized(LocalizationNamespace, "Mineral", "Mineral"),
            }
        );

        Register(
            new EggGroup
            {
                Id = "Amorphous",
                Name = Text.Localized(LocalizationNamespace, "Amorphous", "Amorphous"),
            }
        );

        Register(
            new EggGroup
            {
                Id = "Water2",
                Name = Text.Localized(LocalizationNamespace, "Water2", "Water 2"),
            }
        );

        Register(
            new EggGroup
            {
                Id = "Ditto",
                Name = Text.Localized(LocalizationNamespace, "Ditto", "Ditto"),
                BreedingType = EggGroupType.WithAnyEggGroup,
            }
        );

        Register(
            new EggGroup
            {
                Id = "Dragon",
                Name = Text.Localized(LocalizationNamespace, "Dragon", "Dragon"),
                BreedingType = EggGroupType.WithNoEggGroups,
            }
        );
    }

    #endregion
}
