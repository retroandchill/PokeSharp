using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

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

    #region Defaults

    private const string LocalizationNamespace = "GameData.BattleWeather";

    /// <summary>
    /// Populates the default values for the <see cref="BattleWeather"/> entity.
    /// This method registers various predefined weather conditions, including
    /// Sun, Rain, Sandstorm, Hail, Fog, and others, with their corresponding localized names
    /// to the system.
    /// </summary>
    public static void AddDefaultValues()
    {
        Register(new BattleWeather { Id = "Sun", Name = Text.Localized(LocalizationNamespace, "Sun", "Sun") });

        Register(new BattleWeather { Id = "Rain", Name = Text.Localized(LocalizationNamespace, "Rain", "Rain") });

        Register(
            new BattleWeather
            {
                Id = "Sandstorm",
                Name = Text.Localized(LocalizationNamespace, "Sandstorm", "Sandstorm"),
            }
        );

        Register(new BattleWeather { Id = "Hail", Name = Text.Localized(LocalizationNamespace, "Hail", "Hail") });

        Register(new BattleWeather { Id = "Fog", Name = Text.Localized(LocalizationNamespace, "Fog", "Fog") });

        Register(
            new BattleWeather { Id = "HarshSun", Name = Text.Localized(LocalizationNamespace, "HarshSun", "Harsh Sun") }
        );

        Register(
            new BattleWeather
            {
                Id = "HeavyRain",
                Name = Text.Localized(LocalizationNamespace, "HeavyRain", "Heavy Rain"),
            }
        );

        Register(
            new BattleWeather
            {
                Id = "StrongWinds",
                Name = Text.Localized(LocalizationNamespace, "StrongWinds", "Strong Winds"),
            }
        );

        Register(
            new BattleWeather
            {
                Id = "ShadowSky",
                Name = Text.Localized(LocalizationNamespace, "ShadowSky", "Shadow Sky"),
            }
        );
    }
    #endregion
}
