#if POKESHARP_GENERATOR
using RhoMicro.CodeAnalysis;
#endif

// ReSharper disable once CheckNamespace
namespace PokeSharp.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
#if POKESHARP_GENERATOR
[IncludeFile]
#endif
internal class AutoServiceShortcutAttribute : Attribute
{
    public string? Name { get; init; }
}
