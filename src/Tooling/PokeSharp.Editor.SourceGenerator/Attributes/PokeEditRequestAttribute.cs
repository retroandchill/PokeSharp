#if POKESHARP_EDITOR_GENERATOR
using RhoMicro.CodeAnalysis;
#endif

// ReSharper disable once CheckNamespace
namespace PokeSharp.Editor.Core;

[AttributeUsage(AttributeTargets.Method)]
#if POKESHARP_EDITOR_GENERATOR
[IncludeFile]
#endif
internal class PokeEditRequestAttribute(string? requestName = null) : Attribute
{
    public string? RequestName { get; } = requestName;
}
