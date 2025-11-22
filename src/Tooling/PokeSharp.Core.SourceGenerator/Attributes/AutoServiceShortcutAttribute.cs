#if POKESHARP_CORE_GENERATOR
using RhoMicro.CodeAnalysis;
#endif

// ReSharper disable once CheckNamespace
namespace PokeSharp.Core;

internal enum AutoServiceShortcutType : byte
{
    Default,
    Options,
    GameState,
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
#if POKESHARP_CORE_GENERATOR
[IncludeFile]
#endif
internal class AutoServiceShortcutAttribute : Attribute
{
    public string? Name { get; init; }

    public AutoServiceShortcutType Type { get; init; } = AutoServiceShortcutType.Default;
}
