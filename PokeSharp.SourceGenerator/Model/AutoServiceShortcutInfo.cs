using PokeSharp.SourceGenerator.Attributes;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.SourceGenerator.Model;

[AttributeInfoType<AutoServiceShortcutAttribute>]
public readonly record struct AutoServiceShortcutInfo
{
    public string? Name { get; init; }
}
