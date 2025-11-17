using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;
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
}

[GameDataRegistration<EggGroup>]
[RegisterSingleton<IGameDataProvider<EggGroup>>]
public partial class EggGroupRegistrations
{
    private const string LocalizationNamespace = "GameData.EggGroup";

    [GameDataEntityRegistration]
    internal static readonly EggGroup Undiscovered = new()
    {
        Id = "Undiscovered",
        Name = Text.Localized(LocalizationNamespace, "Undiscovered", "Undiscovered"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Monster = new()
    {
        Id = "Monster",
        Name = Text.Localized(LocalizationNamespace, "Monster", "Monster"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Water1 = new()
    {
        Id = "Water1",
        Name = Text.Localized(LocalizationNamespace, "Water1", "Water 1"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Bug = new()
    {
        Id = "Bug",
        Name = Text.Localized(LocalizationNamespace, "Bug", "Bug"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Flying = new()
    {
        Id = "Flying",
        Name = Text.Localized(LocalizationNamespace, "Flying", "Flying"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Field = new()
    {
        Id = "Field",
        Name = Text.Localized(LocalizationNamespace, "Field", "Field"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Fairy = new()
    {
        Id = "Fairy",
        Name = Text.Localized(LocalizationNamespace, "Fairy", "Fairy"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Grass = new()
    {
        Id = "Grass",
        Name = Text.Localized(LocalizationNamespace, "Grass", "Grass"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Humanlike = new()
    {
        Id = "Humanlike",
        Name = Text.Localized(LocalizationNamespace, "Humanlike", "Humanlike"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Water3 = new()
    {
        Id = "Water3",
        Name = Text.Localized(LocalizationNamespace, "Water3", "Water 3"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Mineral = new()
    {
        Id = "Mineral",
        Name = Text.Localized(LocalizationNamespace, "Mineral", "Mineral"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Amorphous = new()
    {
        Id = "Amorphous",
        Name = Text.Localized(LocalizationNamespace, "Amorphous", "Amorphous"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Water2 = new()
    {
        Id = "Water2",
        Name = Text.Localized(LocalizationNamespace, "Water2", "Water 2"),
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Ditto = new()
    {
        Id = "Ditto",
        Name = Text.Localized(LocalizationNamespace, "Ditto", "Ditto"),
        BreedingType = EggGroupType.WithAnyEggGroup,
    };

    [GameDataEntityRegistration]
    internal static readonly EggGroup Dragon = new()
    {
        Id = "Dragon",
        Name = Text.Localized(LocalizationNamespace, "Dragon", "Dragon"),
        BreedingType = EggGroupType.WithNoEggGroups,
    };
}
