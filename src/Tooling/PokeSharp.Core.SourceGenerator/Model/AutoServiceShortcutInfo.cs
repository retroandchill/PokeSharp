using PokeSharp.Core;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.Core.SourceGenerator.Model;

[AttributeInfoType<AutoServiceShortcutAttribute>]
internal readonly record struct AutoServiceShortcutInfo
{
    public string? Name { get; init; }

    public AutoServiceShortcutType Type { get; init; }
}
