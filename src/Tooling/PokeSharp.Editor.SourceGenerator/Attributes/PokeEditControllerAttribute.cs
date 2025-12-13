#if POKESHARP_EDITOR_GENERATOR
using RhoMicro.CodeAnalysis;
#endif

// ReSharper disable once CheckNamespace
namespace PokeSharp.Editor.Core;

[AttributeUsage(AttributeTargets.Class)]
#if POKESHARP_EDITOR_GENERATOR
[IncludeFile]
#endif
internal class PokeEditControllerAttribute(string? path = null) : Attribute
{
    public string? Path { get; } = path;
}
