using System.Text.RegularExpressions;

namespace PokeSharp.Core.Strings;

/// <summary>
/// A simple text provider that does not perform any localization.
/// </summary>
public sealed partial class UnlocalizedTextProvider : ITextProvider
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

    public ITextData FromLocText(string locString)
    {
        var match = NsLocTextPattern.Match(locString);
        if (!match.Success)
        {
            return new BasicTextData(locString, locString);
        }

        var escapedNs = match.Groups["ns"].Value.Unescape();
        var escapedKey = match.Groups["key"].Value.Unescape();
        var escapedSource = match.Groups["src"].Value.Unescape();
        return new BasicTextData(escapedSource, escapedSource, escapedNs, escapedKey);
    }

    public ITextData FromLocText(ReadOnlySpan<char> locString)
    {
        throw new NotImplementedException();
    }

    [GeneratedRegex(
        """NSLOCTEXT\(\s*"(?<ns>(?:\\.|[^"\\])*)"\s*,\s*"(?<key>(?:\\.|[^"\\])*)"\s*,\s*"(?<src>(?:\\.|[^"\\])*)"\s*\)"""
    )]
    private static partial Regex NsLocTextPattern { get; }
}
