using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

/// <summary>
/// Specifies the type of breeding compatibility based on Pokémon egg groups.
/// </summary>
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

/// <summary>
/// Represents an Egg Group entity in the Pokémon world's ecosystem.
/// An Egg Group determines which Pokémon can breed with each other.
/// </summary>
[GameDataEntity]
public readonly partial record struct EggGroup
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Represents the name of the egg group as a localized text.
    /// </summary>
    public required Text Name { get; init; }

    /// Specifies the breeding type of an egg group, determining compatibility and breeding behavior.
    public EggGroupType BreedingType { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.EggGroup";

    /// <summary>
    /// Adds a predefined set of default EggGroup values to the system by registering them.
    /// This includes various categories such as "Undiscovered," "Monster," "Water1,"
    /// and more, along with localized names for each group.
    /// </summary>
    public static void AddDefaultValues()
    {
        Register(
            new EggGroup
            {
                Id = "Undiscovered",
                Name = Text.Localized(LocalizationNamespace, "Undiscovered", "Undiscovered"),
            }
        );

        Register(new EggGroup { Id = "Monster", Name = Text.Localized(LocalizationNamespace, "Monster", "Monster") });

        Register(new EggGroup { Id = "Water1", Name = Text.Localized(LocalizationNamespace, "Water1", "Water 1") });

        Register(new EggGroup { Id = "Bug", Name = Text.Localized(LocalizationNamespace, "Bug", "Bug") });

        Register(new EggGroup { Id = "Flying", Name = Text.Localized(LocalizationNamespace, "Flying", "Flying") });

        Register(new EggGroup { Id = "Field", Name = Text.Localized(LocalizationNamespace, "Field", "Field") });

        Register(new EggGroup { Id = "Fairy", Name = Text.Localized(LocalizationNamespace, "Fairy", "Fairy") });

        Register(new EggGroup { Id = "Grass", Name = Text.Localized(LocalizationNamespace, "Grass", "Grass") });

        Register(
            new EggGroup { Id = "Humanlike", Name = Text.Localized(LocalizationNamespace, "Humanlike", "Humanlike") }
        );

        Register(new EggGroup { Id = "Water3", Name = Text.Localized(LocalizationNamespace, "Water3", "Water 3") });

        Register(new EggGroup { Id = "Mineral", Name = Text.Localized(LocalizationNamespace, "Mineral", "Mineral") });

        Register(
            new EggGroup { Id = "Amorphous", Name = Text.Localized(LocalizationNamespace, "Amorphous", "Amorphous") }
        );

        Register(new EggGroup { Id = "Water2", Name = Text.Localized(LocalizationNamespace, "Water2", "Water 2") });

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
