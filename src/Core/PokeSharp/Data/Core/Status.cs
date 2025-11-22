using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents a status effect within the game data.
/// </summary>
/// <remarks>
/// The <c>Status</c> structure encapsulates the information necessary to define a status effect,
/// such as its unique identifier and localized name. This struct is immutable and serves as
/// an integral part of the game's data model.
/// </remarks>
[GameDataEntity]
public readonly partial record struct Status
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Gets or sets the localized name of the status effect.
    /// </summary>
    /// <remarks>
    /// This property represents the name of the status effect in a localized format. It is used
    /// to display the name of the status within user-facing interfaces or game data where localization
    /// is necessary.
    /// </remarks>
    public required Text Name { get; init; }
}

[GameDataRegistration<Status>]
[RegisterSingleton<IGameDataProvider<Status>>]
public partial class StatusRegistrations
{
    private const string LocalizationNamespace = "GameData.Status";

    [GameDataEntityRegistration]
    internal static readonly Status SLEEP = new()
    {
        Id = "SLEEP",
        Name = Text.Localized(LocalizationNamespace, "SLEEP", "Sleep"),
    };

    [GameDataEntityRegistration]
    internal static readonly Status POISON = new()
    {
        Id = "POISON",
        Name = Text.Localized(LocalizationNamespace, "POISON", "Poison"),
    };

    [GameDataEntityRegistration]
    internal static readonly Status BURN = new()
    {
        Id = "BURN",
        Name = Text.Localized(LocalizationNamespace, "BURN", "Burn"),
    };

    [GameDataEntityRegistration]
    internal static readonly Status PARALYSIS = new()
    {
        Id = "PARALYSIS",
        Name = Text.Localized(LocalizationNamespace, "PARALYSIS", "Paralysis"),
    };

    [GameDataEntityRegistration]
    internal static readonly Status FROZEN = new()
    {
        Id = "FROZEN",
        Name = Text.Localized(LocalizationNamespace, "FROZEN", "Frozen"),
    };
}
