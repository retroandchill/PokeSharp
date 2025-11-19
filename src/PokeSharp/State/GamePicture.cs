using PokeSharp.Core;
using PokeSharp.Core.Utils;

namespace PokeSharp.State;

public enum PictureOrigin : byte
{
    TopLeft,
    Center,
}

public enum BlendType : byte
{
    Normal,
    Add,
    Subtract,
}

public readonly record struct PicturePosition(float X, float Y, float ZoomX, float ZoomY, float Opacity);

public class GamePicture(int number)
{
    private const float DurationDivisor = 20;

    public int Number { get; } = number;

    public string Name { get; private set; } = "";
    public PictureOrigin Origin { get; private set; } = PictureOrigin.TopLeft;
    private PicturePosition _position = new(0, 0, 1, 1, 255);
    public float X => _position.X;
    public float Y => _position.Y;
    public float ZoomX => _position.ZoomX;
    public float ZoomY => _position.ZoomY;
    public float Opacity => _position.Opacity;
    private TimedChange<PicturePosition>? _move;
    public BlendType BlendType { get; private set; } = BlendType.Normal;
    public Tone Tone { get; private set; }
    private TimedChange<Tone>? _toneChange;
    public float Angle { get; private set; } = 0;
    private float _rotateSpeed;

    public void Show(
        string name,
        PictureOrigin origin,
        float x,
        float y,
        float zoomX,
        float zoomY,
        float opacity,
        BlendType blendType
    )
    {
        Name = name;
        Origin = origin;
        _position = new PicturePosition(x, y, zoomX, zoomY, opacity);
        BlendType = blendType;

        _move = null;
        Tone = new Tone(0, 0, 0);
        _toneChange = null;
        Angle = 0;
        _rotateSpeed = 0;
    }

    public void Move(
        float duration,
        PictureOrigin origin,
        float x,
        float y,
        float zoomX,
        float zoomY,
        float opacity,
        BlendType blendType
    )
    {
        Origin = origin;
        _move = new TimedChange<PicturePosition>(_position, new PicturePosition(x, y, zoomX, zoomY, opacity), duration);
        BlendType = blendType;
        Tone = new Tone(0, 0, 0);
        _toneChange = null;
    }

    public void Rotate(float speed)
    {
        _rotateSpeed = speed;
    }

    public void StartToneChange(Tone tone, float duration)
    {
        if (duration == 0)
        {
            Tone = tone;
            return;
        }

        _toneChange = new TimedChange<Tone>(Tone, tone, duration / DurationDivisor);
    }

    public void Erase()
    {
        Name = "";
    }

    public void Tick(float deltaTime)
    {
        if (string.IsNullOrEmpty(Name))
            return;

        if (_move.HasValue)
        {
            _move = _move.Value with { Timer = _move.Value.Timer + deltaTime };

            _position = new PicturePosition(
                Math.Lerp(_move.Value.Initial.X, _move.Value.Target.X, _move.Value.Duration, _move.Value.Timer),
                Math.Lerp(_move.Value.Initial.Y, _move.Value.Target.Y, _move.Value.Duration, _move.Value.Timer),
                Math.Lerp(_move.Value.Initial.ZoomX, _move.Value.Target.ZoomX, _move.Value.Duration, _move.Value.Timer),
                Math.Lerp(_move.Value.Initial.ZoomY, _move.Value.Target.ZoomY, _move.Value.Duration, _move.Value.Timer),
                Math.Lerp(
                    _move.Value.Initial.Opacity,
                    _move.Value.Target.Opacity,
                    _move.Value.Duration,
                    _move.Value.Timer
                )
            );

            if (_move.Value.Timer >= _move.Value.Duration)
            {
                _move = null;
            }
        }

        if (_toneChange.HasValue)
        {
            _toneChange = _toneChange.Value with { Timer = _toneChange.Value.Timer + deltaTime };
            Tone = new Tone(
                Math.Lerp(
                    _toneChange.Value.Initial.Red,
                    _toneChange.Value.Target.Red,
                    _toneChange.Value.Duration,
                    _toneChange.Value.Timer
                ),
                Math.Lerp(
                    _toneChange.Value.Initial.Green,
                    _toneChange.Value.Target.Green,
                    _toneChange.Value.Duration,
                    _toneChange.Value.Timer
                ),
                Math.Lerp(
                    _toneChange.Value.Initial.Blue,
                    _toneChange.Value.Target.Blue,
                    _toneChange.Value.Duration,
                    _toneChange.Value.Timer
                ),
                Math.Lerp(
                    _toneChange.Value.Initial.Gray,
                    _toneChange.Value.Target.Gray,
                    _toneChange.Value.Duration,
                    _toneChange.Value.Timer
                )
            );

            if (_toneChange.Value.Timer >= _toneChange.Value.Duration)
            {
                _toneChange = null;
            }
        }

        if (_rotateSpeed == 0)
            return;

        Angle += _rotateSpeed * deltaTime * DurationDivisor;
        while (Angle < 0)
        {
            Angle += 360;
        }

        Angle %= 360;
    }
}
