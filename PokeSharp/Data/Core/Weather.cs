using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

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

    #region Defaults

    private const string LocalizationNamespace = "GameData.Weather";

    /// <summary>
    /// Populates the default values for the <see cref="Weather"/> data entity.
    /// This method initializes and registers a predefined set of weather entities,
    /// each with their unique identifiers and localized names.
    /// </summary>
    public static void AddDefaultValues()
    {
        Register(
            new Weather
            {
                Id = "Rain",
                Name = Text.Localized(LocalizationNamespace, "Rain", "Rain"),
                Category = BattleWeather.Rain,
            }
        );

        Register(
            new Weather
            {
                Id = "Storm",
                Name = Text.Localized(LocalizationNamespace, "Storm", "Storm"),
                Category = BattleWeather.Rain,
            }
        );

        Register(
            new Weather
            {
                Id = "Snow",
                Name = Text.Localized(LocalizationNamespace, "Snow", "Snow"),
                Category = BattleWeather.Hail,
            }
        );

        Register(
            new Weather
            {
                Id = "Blizzard",
                Name = Text.Localized(LocalizationNamespace, "Blizzard", "Blizzard"),
                Category = BattleWeather.Hail,
            }
        );

        Register(
            new Weather
            {
                Id = "Sandstorm",
                Name = Text.Localized(LocalizationNamespace, "Sandstorm", "Sandstorm"),
                Category = BattleWeather.Sandstorm,
            }
        );

        Register(
            new Weather
            {
                Id = "HeavyRain",
                Name = Text.Localized(LocalizationNamespace, "HeavyRain", "HeavyRain"),
                Category = BattleWeather.Rain,
            }
        );

        Register(
            new Weather
            {
                Id = "Sun",
                Name = Text.Localized(LocalizationNamespace, "Sun", "Sun"),
                Category = BattleWeather.Sun,
            }
        );

        Register(
            new Weather
            {
                Id = "Fog",
                Name = Text.Localized(LocalizationNamespace, "Fog", "Fog"),
                Category = BattleWeather.Fog,
            }
        );
    }
    #endregion
}
