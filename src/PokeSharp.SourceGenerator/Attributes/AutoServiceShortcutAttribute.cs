#if POKESHARP_GENERATOR
using RhoMicro.CodeAnalysis;
#endif

namespace PokeSharp.SourceGenerator.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
#if POKESHARP_GENERATOR
[IncludeFile]
#endif
internal class AutoServiceShortcutAttribute : Attribute
{
    public string? Name { get; init; }
}
