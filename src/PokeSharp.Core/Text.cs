using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Core.Serialization.Json;
using PokeSharp.Core.Serialization.MessagePack;
#if UNREAL_ENGINE
using UnrealSharp.Core;
#endif

namespace PokeSharp.Core;

/// <summary>
/// Represents a flexible text abstraction that supports integration with different serialization formats
/// and localization options.
/// </summary>
/// <remarks>
/// This struct is compatible with both JSON and MessagePack serialization via
/// <see cref="TextJsonConverter"/> and <see cref="TextMessagePackFormatter"/>.
/// Features include:
/// - Constructing from string, span, or localized values.
/// - Compatible with Unreal Engine's native text handling.
/// - Built-in localization support through a custom factory method.
/// </remarks>
/// <threadsafety>
/// This type is immutable and is safe to use across multiple threads.
/// </threadsafety>
/// <seealso cref="TextJsonConverter"/>
/// <seealso cref="TextMessagePackFormatter"/>
[MessagePackFormatter(typeof(TextMessagePackFormatter))]
[JsonConverter(typeof(TextJsonConverter))]
public readonly struct Text : IEquatable<Text>
{
#if UNREAL_ENGINE
    private readonly FText _text;
#else
    private readonly string _text;
#endif

    /// <summary>
    /// Gets a value indicating whether the text is empty or consists only of whitespace.
    /// </summary>
    /// <remarks>
    /// In environments using Unreal Engine, this property checks against Unreal's native
    /// string handling for emptiness. Otherwise, it determines if the text is null, empty,
    /// or consists solely of whitespace characters.
    /// </remarks>
    /// <returns>
    /// <c>true</c> if the text is empty, null, or contains only whitespace;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <threadsafety>
    /// This property is thread-safe due to the immutable nature of the <see cref="Text"/> struct.
    /// </threadsafety>
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

    /// <summary>
    /// Represents an empty or uninitialized instance of the <see cref="Text"/> struct.
    /// </summary>
    /// <remarks>
    /// This property returns a default <see cref="Text"/> instance that signifies
    /// the absence of text. It is functionally equivalent to an empty or null text
    /// value and can be used as a sentinel value in cases where a no-text state needs
    /// to be represented explicitly.
    /// </remarks>
    /// <returns>
    /// A default <see cref="Text"/> instance.
    /// </returns>
    /// <threadsafety>
    /// This property is thread-safe due to the immutable nature of the <see cref="Text"/> struct.
    /// </threadsafety>
    public static Text None => new();

    /// <summary>
    /// Constructs a new instance of the <see cref="Text"/> struct.
    /// </summary>
    public Text()
    {
#if UNREAL_ENGINE
        _text = FText.None;
#else
        _text = string.Empty;
#endif
    }

#if UNREAL_ENGINE
    /// <summary>
    /// Constructs a new instance of the <see cref="Text"/> struct.
    /// </summary>
    /// <param name="text">The text to initialize the instance with.</param>
    public Text(FText text)
    {
        _text = text;
    }
#endif

    /// <summary>
    /// Constructs a new instance of the <see cref="Text"/> struct.
    /// </summary>
    /// <param name="text">The text to initialize the instance with.</param>
    public Text(ReadOnlySpan<char> text)
    {
#if UNREAL_ENGINE
        _text = new FText(text);
#else
        _text = text.ToString();
#endif
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="Text"/> struct.
    /// </summary>
    /// <param name="text">The text to initialize the instance with.</param>
    public Text(string text)
    {
#if UNREAL_ENGINE
        _text = new FText(text);
#else
        _text = text;
#endif
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="Text"/> struct.
    /// </summary>
    /// <param name="name">The name to initialize the instance with.</param>
    public Text(Name name)
    {
#if UNREAL_ENGINE
        _text = new FText((FName)name);
#else
        _text = name.ToString();
#endif
    }

    /// <summary>
    /// Creates a localized <see cref="Text"/> instance associated with the specified namespace, key, and value.
    /// </summary>
    /// <param name="ns">The namespace associated with the localized text.</param>
    /// <param name="key">The key used to identify the localized text.</param>
    /// <param name="value">The localized value for the provided key.</param>
    /// <returns>The constructed <see cref="Text"/> instance containing the localized value.</returns>
    public static Text Localized(ReadOnlySpan<char> ns, ReadOnlySpan<char> key, ReadOnlySpan<char> value)
    {
        // TODO: We need to implement the native localization API
        return new Text(value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Text other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(Text other)
    {
        return this == other;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return _text.GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
#if UNREAL_ENGINE
        return _text.ToString();
#else
        return _text;
#endif
    }

    /// <summary>
    /// Returns the internal text data as a read-only span of characters.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{Char}"/> representing the content of the text.</returns>
    public ReadOnlySpan<char> AsReadOnlySpan()
    {
#if UNREAL_ENGINE
        return _text.AsReadOnlySpan();
#else
        return _text.AsSpan();
#endif
    }

#if UNREAL_ENGINE
    /// <summary>
    /// Implicitly converts a <see cref="FText"/> instance to a <see cref="Text"/> instance.
    /// </summary>
    /// <param name="value">The <see cref="FText"/> instance to convert.</param>
    /// <returns>A new <see cref="Text"/> instance containing the converted value.</returns>
    public static implicit operator Text(FText value) => new(value);

    /// <summary>
    /// Implicitly converts a <see cref="Text"/> instance to a <see cref="FText"/> instance.
    /// </summary>
    /// <param name="value">The <see cref="Text"/> instance to convert.</param>
    /// <returns>A new <see cref="FText"/> instance containing the converted value.</returns>
    public static implicit operator FText(Text value) => value._text;
#endif

    /// <summary>
    /// Allows implicit conversion from a <see cref="string"/> to a <see cref="Text"/> instance.
    /// </summary>
    /// <param name="value">The string to convert to a <see cref="Text"/> instance.</param>
    /// <returns>A new <see cref="Text"/> instance initialized with the specified string.</returns>
    public static implicit operator Text(string value) => new(value);

    /// <summary>
    /// Defines an implicit conversion from <see cref="Text"/> to a <see cref="string"/>.
    /// The string representation of the <see cref="Text"/> instance is returned.
    /// </summary>
    /// <param name="value">The <see cref="Text"/> instance to convert.</param>
    /// <returns>The underlying string value of the <see cref="Text"/> instance.</returns>
    public static implicit operator string(Text value) => value._text;

    /// <summary>
    /// Converts an instance of <see cref="Text"/> to a read-only character span.
    /// </summary>
    /// <param name="value">The <see cref="Text"/> instance to be converted.</param>
    /// <returns>A read-only span of characters representing the text.</returns>
    public static implicit operator ReadOnlySpan<char>(Text value) => value.AsReadOnlySpan();

    /// <summary>
    /// Compares two <see cref="Text"/> instances for equality.
    /// </summary>
    /// <param name="lhs">The first <see cref="Text"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="Text"/> instance to compare.</param>
    /// <returns><c>true</c> if both <see cref="Text"/> instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Text lhs, Text rhs)
    {
        return lhs._text == rhs._text;
    }

    /// <summary>
    /// Determines whether two <see cref="Text"/> instances are not equal.
    /// </summary>
    /// <param name="lhs">The first <see cref="Text"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="Text"/> instance to compare.</param>
    /// <returns>
    /// <c>true</c> if the two instances are not equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(Text lhs, Text rhs)
    {
        return !(lhs == rhs);
    }
}
