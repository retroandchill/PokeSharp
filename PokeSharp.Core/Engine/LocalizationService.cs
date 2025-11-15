using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Core.Engine;

[RegisterSingleton]
[AutoServiceShortcut]
public class LocalizationService
{
    // For now just stup this to English
    public Name Language => "en";
}
