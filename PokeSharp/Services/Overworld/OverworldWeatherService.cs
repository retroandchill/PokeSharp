using PokeSharp.Abstractions;
using PokeSharp.Data.Core;

namespace PokeSharp.Services.Overworld;

public class OverworldWeatherService
{
    public static OverworldWeatherService Instance { get; } = new();

    public Name WeatherType { get; }

    public Weather? Weather => Weather.TryGet(WeatherType, out var weather) ? weather : null;
}
