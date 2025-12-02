using Microsoft.CodeAnalysis;
using PokeSharp.Editor.Core;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.Editor.SourceGenerator.Model;

[AttributeInfoType(typeof(EditableTypeAttribute<>))]
public readonly record struct EditableTypeInfo(ITypeSymbol Type, string? Name = null);
