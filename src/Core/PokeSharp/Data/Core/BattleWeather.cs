using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents the weather conditions during a battle in the game.
/// </summary>
/// <remarks>
/// The <c>BattleWeather</c> struct is used to define possible weather effects in the game's battle system.
/// These conditions may influence various aspects of gameplay, such as move effects, abilities, or stats.
/// </remarks>
[GameDataEntity]
public readonly partial record struct BattleWeather
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Represents the localized name associated with the battle weather condition.
    /// </summary>
    public required Text Name { get; init; }
}

[GameDataRegistration<BattleWeather>]
[RegisterSingleton<IGameDataProvider<BattleWeather>>]
public partial class BattleWeatherRegistrations
{
    private const string LocalizationNamespace = "GameData.BattleWeather";

    [GameDataEntityRegistration]
    internal static readonly BattleWeather Sun = new()
    {
        Id = "Sun",
        Name = Text.Localized(LocalizationNamespace, "Sun", "Sun"),
    };

    [GameDataEntityRegistration]
    internal static readonly BattleWeather Rain = new()
    {
        Id = "Rain",
        Name = Text.Localized(LocalizationNamespace, "Rain", "Rain"),
    };

    [GameDataEntityRegistration]
    internal static readonly BattleWeather Sandstorm = new()
    {
        Id = "Sandstorm",
        Name = Text.Localized(LocalizationNamespace, "Sandstorm", "Sandstorm"),
    };

    [GameDataEntityRegistration]
    internal static readonly BattleWeather Hail = new()
    {
        Id = "Hail",
        Name = Text.Localized(LocalizationNamespace, "Hail", "Hail"),
    };

    [GameDataEntityRegistration]
    internal static readonly BattleWeather Fog = new()
    {
        Id = "Fog",
        Name = Text.Localized(LocalizationNamespace, "Fog", "Fog"),
    };

    [GameDataEntityRegistration]
    internal static readonly BattleWeather HarshSun = new()
    {
        Id = "HarshSun",
        Name = Text.Localized(LocalizationNamespace, "HarshSun", "Harsh Sun"),
    };

    [GameDataEntityRegistration]
    internal static readonly BattleWeather HeavyRain = new()
    {
        Id = "HeavyRain",
        Name = Text.Localized(LocalizationNamespace, "HeavyRain", "Heavy Rain"),
    };

    [GameDataEntityRegistration]
    internal static readonly BattleWeather StrongWinds = new()
    {
        Id = "StrongWinds",
        Name = Text.Localized(LocalizationNamespace, "StrongWinds", "Strong Winds"),
    };

    [GameDataEntityRegistration]
    internal static readonly BattleWeather ShadowSky = new()
    {
        Id = "ShadowSky",
        Name = Text.Localized(LocalizationNamespace, "ShadowSky", "Shadow Sky"),
    };
}
