#if POKESHARP_EDITOR_GENERATOR
using RhoMicro.CodeAnalysis;
#endif

// ReSharper disable once CheckNamespace
namespace PokeSharp.Editor.Core;

[AttributeUsage(AttributeTargets.Class)]
#if POKESHARP_EDITOR_GENERATOR
[IncludeFile]
#endif
internal class PokeEditControllerAttribute(string? name = null) : Attribute
{
    public string? Name { get; } = name;
}
