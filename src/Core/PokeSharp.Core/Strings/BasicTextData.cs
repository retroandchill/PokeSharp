namespace PokeSharp.Core.Strings;

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
    public ReadOnlySpan<char> AsDisplaySpan()
    {
        return DisplayString.AsSpan();
    }

    public string ToLocString()
    {
        if (Namespace is null || Key is null || IsCultureInvariant) return SourceString;
        
        var escapedNs     = Namespace.Escape();
        var escapedKey    = Key.Escape();
        var escapedSource = SourceString.Escape();

        return $"NSLOCTEXT(\"{escapedNs}\", \"{escapedKey}\", \"{escapedSource}\")";

    }
}
