using PokeSharp.Abstractions;
using PokeSharp.Data.Core;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Services.Overworld;

[AutoServiceShortcut]
public class OverworldWeatherService
{
    public Name WeatherType { get; }

    public Weather? Weather => Weather.TryGet(WeatherType, out var weather) ? weather : null;
}
