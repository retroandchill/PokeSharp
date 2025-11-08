using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.HardCoded;

[GameDataEntity]
public partial record Weather
{

    public required Name Id { get; init; }

    public required Text Name { get; init; }
    
    public required Name Category { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.Weather";

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
