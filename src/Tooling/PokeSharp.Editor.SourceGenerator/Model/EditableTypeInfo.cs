using PokeSharp.Editor.Core;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.Editor.SourceGenerator.Model;

[AttributeInfoType<EditableTypeAttribute>]
public readonly record struct EditableTypeInfo(string? Name);
