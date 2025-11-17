using PokeSharp.Core;
using PokeSharp.Data.Core;

namespace PokeSharp.Services.Overworld;

[AutoServiceShortcut]
public class OverworldWeatherService
{
    public Name WeatherType { get; }

    public Weather? Weather => Weather.TryGet(WeatherType, out var weather) ? weather : null;
}
