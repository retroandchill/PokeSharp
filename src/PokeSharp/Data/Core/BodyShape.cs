using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents a body shape entity that is part of the game's data.
/// </summary>
/// <remarks>
/// A body shape is identified by an <see cref="Id"/> and has a localized <see cref="Name"/>.
/// It is used to categorize entities or characters within the game based on their physical forms.
/// </remarks>
/// <threadsafety>
/// This type is immutable and thread-safe since its properties are readonly once initialized.
/// </threadsafety>
/// <example>
/// The AddDefaultValues method registers a list of predefined body shapes with localized names.
/// </example>
[GameDataEntity]
public readonly partial record struct BodyShape
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Gets the localized name of the body shape.
    /// </summary>
    /// <remarks>
    /// The <see cref="Name"/> property provides a text representation that is localized
    /// to match the player's language settings, enhancing accessibility and comprehension.
    /// </remarks>
    /// <threadsafety>
    /// This property is immutable and thread-safe as it is readonly after initialization.
    /// </threadsafety>
    public required Text Name { get; init; }
}

[GameDataRegistration<BodyShape>]
[RegisterSingleton<IGameDataProvider<BodyShape>>]
public partial class BodyShapeRegistrations
{
    private const string LocalizationNamespace = "GameData.BodyShape";

    [GameDataEntityRegistration]
    internal static readonly BodyShape Head = new()
    {
        Id = "Head",
        Name = Text.Localized(LocalizationNamespace, "Head", "Head"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape Serpentine = new()
    {
        Id = "Serpentine",
        Name = Text.Localized(LocalizationNamespace, "Serpentine", "Serpentine"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape Finned = new()
    {
        Id = "Finned",
        Name = Text.Localized(LocalizationNamespace, "Finned", "Finned"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape HeadArms = new()
    {
        Id = "HeadArms",
        Name = Text.Localized(LocalizationNamespace, "HeadArms", "Head and arms"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape HeadBase = new()
    {
        Id = "HeadBase",
        Name = Text.Localized(LocalizationNamespace, "HeadBase", "Head and base"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape BipedalTail = new()
    {
        Id = "BipedalTail",
        Name = Text.Localized(LocalizationNamespace, "BipedalTail", "Bipedal with tail"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape HeadLegs = new()
    {
        Id = "HeadLegs",
        Name = Text.Localized(LocalizationNamespace, "HeadLegs", "Head and legs"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape Quadruped = new()
    {
        Id = "Quadruped",
        Name = Text.Localized(LocalizationNamespace, "Quadruped", "Quadruped"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape Winged = new()
    {
        Id = "Winged",
        Name = Text.Localized(LocalizationNamespace, "Winged", "Winged"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape Multiped = new()
    {
        Id = "Multiped",
        Name = Text.Localized(LocalizationNamespace, "Multiped", "Multiped"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape MultiBody = new()
    {
        Id = "MultiBody",
        Name = Text.Localized(LocalizationNamespace, "MultiBody", "Multi Body"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape Bipedal = new()
    {
        Id = "Bipedal",
        Name = Text.Localized(LocalizationNamespace, "Bipedal", "Bipedal"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape MultiWinged = new()
    {
        Id = "MultiWinged",
        Name = Text.Localized(LocalizationNamespace, "MultiWinged", "Multi Winged"),
    };

    [GameDataEntityRegistration]
    internal static readonly BodyShape Insectoid = new()
    {
        Id = "Insectoid",
        Name = Text.Localized(LocalizationNamespace, "Insectoid", "Insectoid"),
    };
}
