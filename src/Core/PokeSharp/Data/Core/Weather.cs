using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents a weather condition in the game. Each weather is identified
/// by a unique ID and has a display name. Weather types are also categorized
/// into specific groups, such as rain, hail, or sandstorm.
/// </summary>
/// <remarks>
/// The <c>Weather</c> class provides functionality to register predefined
/// weather conditions, which can be used to simulate various environmental
/// effects in gameplay scenarios.
/// </remarks>
[GameDataEntity]
public partial record Weather
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Gets the localized name of the weather condition.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// The category of the weather condition.
    /// </summary>
    public required Name Category { get; init; }
}

[GameDataRegistration<Weather>]
[RegisterSingleton<IGameDataProvider<Weather>>]
public partial class WeatherRegistrations
{
    private const string LocalizationNamespace = "GameData.Weather";

    [GameDataEntityRegistration]
    internal static readonly Weather Rain = new()
    {
        Id = "Rain",
        Name = Text.Localized(LocalizationNamespace, "Rain", "Rain"),
        Category = BattleWeather.Rain.Id,
    };

    [GameDataEntityRegistration]
    internal static readonly Weather Storm = new()
    {
        Id = "Storm",
        Name = Text.Localized(LocalizationNamespace, "Storm", "Storm"),
        Category = BattleWeather.Rain.Id,
    };

    [GameDataEntityRegistration]
    internal static readonly Weather Snow = new()
    {
        Id = "Snow",
        Name = Text.Localized(LocalizationNamespace, "Snow", "Snow"),
        Category = BattleWeather.Hail.Id,
    };

    [GameDataEntityRegistration]
    internal static readonly Weather Blizzard = new()
    {
        Id = "Blizzard",
        Name = Text.Localized(LocalizationNamespace, "Blizzard", "Blizzard"),
        Category = BattleWeather.Hail.Id,
    };

    [GameDataEntityRegistration]
    internal static readonly Weather Sandstorm = new()
    {
        Id = "Sandstorm",
        Name = Text.Localized(LocalizationNamespace, "Sandstorm", "Sandstorm"),
        Category = BattleWeather.Sandstorm.Id,
    };

    [GameDataEntityRegistration]
    internal static readonly Weather HeavyRain = new()
    {
        Id = "HeavyRain",
        Name = Text.Localized(LocalizationNamespace, "HeavyRain", "HeavyRain"),
        Category = BattleWeather.Rain.Id,
    };

    [GameDataEntityRegistration]
    internal static readonly Weather Sun = new()
    {
        Id = "Sun",
        Name = Text.Localized(LocalizationNamespace, "Sun", "Sun"),
        Category = BattleWeather.Sun.Id,
    };

    [GameDataEntityRegistration]
    internal static readonly Weather Fog = new()
    {
        Id = "Fog",
        Name = Text.Localized(LocalizationNamespace, "Fog", "Fog"),
        Category = BattleWeather.Fog.Id,
    };
}
