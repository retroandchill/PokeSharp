using PokeSharp.Core.Strings;

namespace PokeSharp.Unreal.Core.Strings;

internal class UnrealTextProvider : ITextProvider
{
    public ITextData FromSimpleString(string text)
    {
        return FromSimpleString(text.AsSpan());
    }

    public ITextData FromSimpleString(ReadOnlySpan<char> text)
    {
        return new UnrealTextData(text);
    }

    public ITextData FromLocalized(string ns, string key, string value)
    {
        return FromLocalized(ns.AsSpan(), key.AsSpan(), value.AsSpan());
    }

    public ITextData FromLocalized(ReadOnlySpan<char> ns, ReadOnlySpan<char> key, ReadOnlySpan<char> value)
    {
        return new UnrealTextData(ns, key, value);
    }

    public ITextData FromLocText(string locString)
    {
        return FromLocText(locString.AsSpan());
    }

    public ITextData FromLocText(ReadOnlySpan<char> locString)
    {
        return UnrealTextData.FromLocText(locString);
    }
}
