namespace PokeSharp.Core.Strings;

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

    static void ResetTextProvider() => Instance = new UnlocalizedTextProvider();

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
    
    ITextData FromLocText(string locString);
    
    ITextData FromLocText(ReadOnlySpan<char> locString);
}
