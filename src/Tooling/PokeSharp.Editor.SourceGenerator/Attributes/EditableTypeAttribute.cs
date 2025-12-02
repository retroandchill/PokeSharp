#if POKESHARP_EDITOR_GENERATOR
using RhoMicro.CodeAnalysis;
#else
using PokeSharp.Core.Data;
#endif

// ReSharper disable once CheckNamespace
namespace PokeSharp.Editor.Core;

/// <summary>
/// Indicates that a class is a fully editable type, that can be edited within the editor.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
#if POKESHARP_EDITOR_GENERATOR
[IncludeFile]
#endif
internal class EditableTypeAttribute<T>(string? name = null) : Attribute
{
    public string? Name { get; } = name;
}
