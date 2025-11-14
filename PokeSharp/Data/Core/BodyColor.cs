using System.Collections;
using Injectio.Attributes;
using PokeSharp.Abstractions;
using PokeSharp.Core.Data;
using PokeSharp.Data.Core;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents a body color entity used in the PokeSharp data framework.
/// This struct is a part of game data entities and is used to define colors
/// associated with various in-game elements.
/// </summary>
/// <remarks>
/// The <see cref="BodyColor"/> struct is a read-only record type, making it immutable once initialized.
/// It includes a unique identifier and a display name, which are essential for its representation.
/// </remarks>
[GameDataEntity]
public readonly partial record struct BodyColor
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Represents a localized name for the corresponding body color.
    /// </summary>
    public required Text Name { get; init; }
}

[GameDataRegistration<BodyColor>]
[RegisterSingleton<IGameDataProvider<BodyColor>>]
public partial class BodyColorRegistrations
{
    private const string LocalizationNamespace = "GameData.BodyColor";

    [GameDataEntityRegistration]
    internal static readonly BodyColor Red = new()
    {
        Id = "Red",
        Name = Text.Localized(LocalizationNamespace, "Red", "Red"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyColor Blue = new()
    {
        Id = "Blue",
        Name = Text.Localized(LocalizationNamespace, "Blue", "Blue"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyColor Yellow = new()
    {
        Id = "Yellow",
        Name = Text.Localized(LocalizationNamespace, "Yellow", "Yellow"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyColor Green = new()
    {
        Id = "Green",
        Name = Text.Localized(LocalizationNamespace, "Green", "Green"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyColor Black = new()
    {
        Id = "Black",
        Name = Text.Localized(LocalizationNamespace, "Black", "Black"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyColor Brown = new()
    {
        Id = "Brown",
        Name = Text.Localized(LocalizationNamespace, "Brown", "Brown"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyColor Purple = new()
    {
        Id = "Purple",
        Name = Text.Localized(LocalizationNamespace, "Purple", "Purple"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyColor Gray = new()
    {
        Id = "Gray",
        Name = Text.Localized(LocalizationNamespace, "Gray", "Gray"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyColor White = new()
    {
        Id = "White",
        Name = Text.Localized(LocalizationNamespace, "White", "White"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyColor Pink = new()
    {
        Id = "Pink",
        Name = Text.Localized(LocalizationNamespace, "Pink", "Pink"),
    };
}
