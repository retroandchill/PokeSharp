using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

[GameDataEntity]
public readonly partial record struct BattleWeather
{

    public required Name Id { get; init; }

    public required Text Name { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.BattleWeather";

    public static void AddDefaultValues()
    {
        Register(
            new BattleWeather
            {
                Id = "Sun",
                Name = Text.Localized(LocalizationNamespace, "Sun", "Sun"),
            }
        );

        Register(
            new BattleWeather
            {
                Id = "Rain",
                Name = Text.Localized(LocalizationNamespace, "Rain", "Rain"),
            }
        );

        Register(
            new BattleWeather
            {
                Id = "Sandstorm",
                Name = Text.Localized(LocalizationNamespace, "Sandstorm", "Sandstorm"),
            }
        );

        Register(
            new BattleWeather
            {
                Id = "Hail",
                Name = Text.Localized(LocalizationNamespace, "Hail", "Hail"),
            }
        );

        Register(
            new BattleWeather
            {
                Id = "Fog",
                Name = Text.Localized(LocalizationNamespace, "Fog", "Fog"),
            }
        );

        Register(
            new BattleWeather
            {
                Id = "HarshSun",
                Name = Text.Localized(LocalizationNamespace, "HarshSun", "Harsh Sun"),
            }
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
