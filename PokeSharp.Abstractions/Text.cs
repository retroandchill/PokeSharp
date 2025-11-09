using PokeSharp.Abstractions;
#if UNREAL_ENGINE
using UnrealSharp.Core;
#endif

namespace PokeSharp.Abstractions;

public readonly struct Text : IEquatable<Text>
{
#if UNREAL_ENGINE
    private readonly FText _text;
#else
    private readonly string _text;
#endif

    public bool Empty
    {
        get
        {
#if UNREAL_ENGINE
            return _text.Empty;
#else
            return string.IsNullOrWhiteSpace(_text);
#endif
        }
    }

    public static Text None => new();

    public Text()
    {
#if UNREAL_ENGINE
        _text = FText.None;
#else
        _text = string.Empty;
#endif
    }

    public Text(ReadOnlySpan<char> text)
    {
#if UNREAL_ENGINE
        _text = new FText(text);
#else
        _text = text.ToString();
#endif
    }

    public Text(string text)
    {
#if UNREAL_ENGINE
        _text = new FText(text);
#else
        _text = text;
#endif
    }

    public Text(Name name)
    {
#if UNREAL_ENGINE
        _text = new FText((FName)name);
#else
        _text = name.ToString();
#endif
    }

    public static Text Localized(
        ReadOnlySpan<char> ns,
        ReadOnlySpan<char> key,
        ReadOnlySpan<char> value
    )
    {
        // TODO: We need to implement the native localization API
        return new Text(value);
    }

    public override bool Equals(object? obj)
    {
        return obj is Text other && Equals(other);
    }

    public bool Equals(Text other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return _text.GetHashCode();
    }

    public override string ToString()
    {
#if UNREAL_ENGINE
        return _text.ToString();
#else
        return _text;
#endif
    }

    public ReadOnlySpan<char> AsReadOnlySpan()
    {
#if UNREAL_ENGINE
        return _text.AsReadOnlySpan();
#else
        return _text.AsSpan();
#endif
    }

    public static implicit operator Text(string value) => new(value);

    public static implicit operator string(Text value) => value._text;

    public static implicit operator ReadOnlySpan<char>(Text value) => value.AsReadOnlySpan();

    public static bool operator ==(Text lhs, Text rhs)
    {
        return lhs._text == rhs._text;
    }

    public static bool operator !=(Text lhs, Text rhs)
    {
        return !(lhs == rhs);
    }
}
