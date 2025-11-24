using PokeSharp.Core.Strings;

namespace PokeSharp.Core.Engine;

[RegisterSingleton]
[AutoServiceShortcut]
public class LocalizationService
{
    // For now just stup this to English
    public Name Language => "en";
}
