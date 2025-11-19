using System.Drawing;
using PokeSharp.Core;

namespace PokeSharp.State;

public readonly record struct TimedChange<T>(T Initial, T Target, float Duration, float Timer = 0);

public readonly record struct TimedValue<T>(T Value, float Duration, float Timer = 0);

public readonly record struct Shake(float Power, float Speed, float Duration, float Timer = 0);

public class GameScreen
{
    public float Brightness { get; private set; }
    public Tone Tone { get; private set; }
    public Color FlashColor { get; private set; }
    public float Shake { get; private set; }
    public List<GamePicture> Pictures { get; private set; }
    public Name WeatherType { get; private set; }
    public float WeatherMax { get; private set; }
    public float WeatherDuration { get; private set; }

    private TimedChange<Tone>? _toneChange;
    private TimedValue<float>? _flashChange;
}
