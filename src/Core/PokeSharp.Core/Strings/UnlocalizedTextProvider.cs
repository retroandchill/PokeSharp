namespace PokeSharp.Core.Strings;

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
