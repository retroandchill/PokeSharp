namespace PokeSharp.Core.Strings;

/// <summary>
/// Basic abstraction unit for text data.
/// </summary>
public interface ITextData
{
    /// <summary>
    /// Original source string (authoring-time text, typically in English).
    /// </summary>
    string? SourceString { get; }

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
    /// Returns the display string as a read-only span. This typically can just get the span for <see cref="DisplayString"/>,
    /// but if your implementation stores the string in native memory, this can be used to avoid the costs of copying
    /// the string data to managed memory.
    /// </summary>
    /// <returns></returns>
    ReadOnlySpan<char> AsDisplaySpan();
}
