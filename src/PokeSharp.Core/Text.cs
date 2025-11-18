using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Core.Serialization.Json;
using PokeSharp.Core.Serialization.MessagePack;

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
    private readonly ITextData? _data;

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
    public bool IsEmpty => _data is null || _data.IsEmpty;

    /// <summary>
    /// Gets a value indicating whether the text consists solely of whitespace characters.
    /// </summary>
    public bool IsWhitespace => _data is not null && _data.IsWhitespace;

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
    /// <param name="text">The text to initialize the instance with.</param>
    public Text(ReadOnlySpan<char> text)
    {
        _data = ITextProvider.Instance.FromSimpleString(text);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="Text"/> struct.
    /// </summary>
    /// <param name="text">The text to initialize the instance with.</param>
    public Text(string text)
    {
        _data = ITextProvider.Instance.FromSimpleString(text);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="Text"/> struct.
    /// </summary>
    /// <param name="name">The name to initialize the instance with.</param>
    public Text(Name name)
    {
        _data = ITextProvider.Instance.FromSimpleString(name.ToString());
    }

    private Text(ITextData data)
    {
        _data = data;
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
        return new Text(ITextProvider.Instance.FromLocalized(ns, key, value));
    }

    /// <summary>
    /// Creates a localized <see cref="Text"/> instance associated with the specified namespace, key, and value.
    /// </summary>
    /// <param name="ns">The namespace associated with the localized text.</param>
    /// <param name="key">The key used to identify the localized text.</param>
    /// <param name="value">The localized value for the provided key.</param>
    /// <returns>The constructed <see cref="Text"/> instance containing the localized value.</returns>
    public static Text Localized(string ns, string key, string value)
    {
        return new Text(ITextProvider.Instance.FromLocalized(ns, key, value));
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
        return _data?.GetHashCode() ?? 0;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return _data?.DisplayString ?? string.Empty;
    }

    /// <summary>
    /// Returns the internal text data as a read-only span of characters.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{Char}"/> representing the content of the text.</returns>
    public ReadOnlySpan<char> AsReadOnlySpan()
    {
        return _data is not null ? _data.AsDisplaySpan() : ReadOnlySpan<char>.Empty;
    }

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
    public static implicit operator string(Text value) => value._data?.DisplayString ?? string.Empty;

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
        return Equals(lhs._data, rhs._data);
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

/// <summary>
/// Basic abstraction unit for text data.
/// </summary>
public interface ITextData
{
    /// <summary>
    /// Original source string (authoring-time text, typically in English).
    /// </summary>
    string SourceString { get; }

    /// <summary>
    /// Current display string, after localization / transformation.
    /// For simple invariant text, this can just be SourceString.
    /// </summary>
    string DisplayString { get; }

    /// <summary>
    /// Optional namespace used for localization lookup.
    /// Null or empty means "no namespace".
    /// </summary>
    string? Namespace { get; }

    /// <summary>
    /// Optional key used for localization lookup.
    /// Null or empty means "no key".
    /// </summary>
    string? Key { get; }

    /// <summary>
    /// True if this text should not be localized (culture invariant).
    /// </summary>
    bool IsCultureInvariant { get; }

    /// <summary>
    /// True if this text is transient (not meant to be gathered/serialized for localization).
    /// </summary>
    bool IsTransient { get; }

    /// <summary>
    /// True if this text should be gathered for localization (rough equivalent of UE ShouldGatherForLocalization()).
    /// </summary>
    bool ShouldGatherForLocalization { get; }

    /// <summary>
    /// True if the text is empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// True if the text is empty or consists solely of whitespace characters.
    /// </summary>
    bool IsWhitespace { get; }

    /// <summary>
    /// Returns a copy of this data with a different display string.
    /// Useful for transformations like ToUpper/ToLower without losing the identity.
    /// </summary>
    ITextData WithDisplayString(string newDisplay);

    /// <summary>
    /// Returns a copy of this data with different namespace/key (for ChangeKey‑style operations).
    /// </summary>
    ITextData WithIdentity(string? @namespace, string? key);

    /// <summary>
    /// Returns the display string as a read-only span. This typically can just get the span for <see cref="DisplayString"/>,
    /// but if your implementation stores the string in native memory, this can be used to avoid the costs of copying
    /// the string data to managed memory.
    /// </summary>
    /// <returns></returns>
    ReadOnlySpan<char> AsDisplaySpan();
}

/// <summary>
/// Basic implementation of <see cref="ITextData"/> that stores the original source string and display string. This
/// version will not update in real time when the display string changes, and thus is not suitable for localization
/// where the user can change the language at runtime.
/// </summary>
/// <param name="SourceString">Original source string (authoring-time text, typically in English).</param>
/// <param name="DisplayString">Current display string, after localization / transformation.</param>
/// <param name="Namespace">Optional namespace used for localization lookup.</param>
/// <param name="Key">Optional key used for localization lookup.</param>
/// <param name="IsCultureInvariant">True if this text should not be localized (culture invariant).</param>
/// <param name="IsTransient">True if this text is transient (not meant to be gathered/serialized for localization).</param>
/// <param name="ShouldGatherForLocalization">True if this text should be gathered for localization (rough equivalent of UE ShouldGatherForLocalization()).</param>
public sealed record BasicTextData(
    string SourceString,
    string DisplayString,
    string? Namespace = null,
    string? Key = null,
    bool IsCultureInvariant = false,
    bool IsTransient = false,
    bool ShouldGatherForLocalization = false
) : ITextData
{
    /// <inheritdoc />
    public bool IsEmpty => string.IsNullOrEmpty(DisplayString);

    /// <inheritdoc />
    public bool IsWhitespace => string.IsNullOrWhiteSpace(DisplayString);

    /// <inheritdoc />
    public ITextData WithDisplayString(string newDisplay)
    {
        return this with { DisplayString = newDisplay };
    }

    /// <inheritdoc />
    public ITextData WithIdentity(string? @namespace, string? key)
    {
        return this with { Namespace = @namespace, Key = key };
    }

    /// <inheritdoc />
    public ReadOnlySpan<char> AsDisplaySpan()
    {
        return DisplayString.AsSpan();
    }
}

/// <summary>
/// Abstract singleton provider for text data. By default, uses <see cref="UnlocalizedTextProvider"/>, but can be
/// changed to use a different implementation.
/// </summary>
public interface ITextProvider
{
    /// <summary>
    /// The current instance of the text provider.
    /// </summary>
    static ITextProvider Instance { get; internal set; } = new UnlocalizedTextProvider();

    /// <summary>
    /// Sets the current instance of the text provider to the specified provider.
    /// </summary>
    /// <param name="provider">The new provider to use</param>
    static void UseCustomProvider(ITextProvider provider)
    {
        Instance = provider;
    }

    /// <summary>
    /// Creates a new <see cref="ITextData"/> instance from the given text. This text will not have any localization
    /// keys, and thus will not be localized.
    /// </summary>
    /// <param name="text">The text to use</param>
    /// <returns>The text data used by the Text struct</returns>
    ITextData FromSimpleString(string text);

    /// <summary>
    /// Creates a new <see cref="ITextData"/> instance from the given text. This text will not have any localization
    /// keys, and thus will not be localized.
    /// </summary>
    /// <param name="text">The text to use</param>
    /// <returns>The text data used by the Text struct</returns>
    ITextData FromSimpleString(ReadOnlySpan<char> text);

    /// <summary>
    /// Creates a new <see cref="ITextData"/> instance from the given namespace, key, and value.
    /// </summary>
    /// <param name="ns">The localization namespace.</param>
    /// <param name="key">The localization key.</param>
    /// <param name="value">The value of the string.</param>
    /// <returns>The text data that can be polled.</returns>
    ITextData FromLocalized(string ns, string key, string value);

    /// <summary>
    /// Creates a new <see cref="ITextData"/> instance from the given namespace, key, and value.
    /// </summary>
    /// <param name="ns">The localization namespace.</param>
    /// <param name="key">The localization key.</param>
    /// <param name="value">The value of the string.</param>
    /// <returns>The text data that can be polled.</returns>
    ITextData FromLocalized(ReadOnlySpan<char> ns, ReadOnlySpan<char> key, ReadOnlySpan<char> value);
}

/// <summary>
/// A simple text provider that does not perform any localization.
/// </summary>
public sealed class UnlocalizedTextProvider : ITextProvider
{
    /// <inheritdoc />
    public ITextData FromSimpleString(string text)
    {
        return new BasicTextData(text, text);
    }

    /// <inheritdoc />
    public ITextData FromSimpleString(ReadOnlySpan<char> text)
    {
        var asString = text.ToString();
        return new BasicTextData(asString, asString);
    }

    /// <inheritdoc />
    public ITextData FromLocalized(string ns, string key, string value)
    {
        return new BasicTextData(value, value, ns, key);
    }

    /// <inheritdoc />
    public ITextData FromLocalized(ReadOnlySpan<char> ns, ReadOnlySpan<char> key, ReadOnlySpan<char> value)
    {
        var valueAsString = value.ToString();
        return new BasicTextData(valueAsString, valueAsString, ns.ToString(), key.ToString());
    }
}
