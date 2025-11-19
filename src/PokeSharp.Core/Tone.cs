namespace PokeSharp.Core;

public readonly record struct Tone
{
    public float Red { get; }

    public float Green { get; }
    public float Blue { get; }
    public float Gray { get; }

    public Tone(float red, float green, float blue, float gray = 0)
    {
        Red = Clamp(red, -255, 255);
        Green = Clamp(green, -255, 255);
        Blue = Clamp(blue, -255, 255);
        Gray = Clamp(gray, 0, 255);
    }

    private static float Clamp(float value, float min, float max)
    {
        return value < min ? min
            : value > max ? max
            : value;
    }

    public void Deconstruct(out float red, out float green, out float blue, out float gray)
    {
        red = Red;
        green = Green;
        blue = Blue;
        gray = Gray;
    }
}
