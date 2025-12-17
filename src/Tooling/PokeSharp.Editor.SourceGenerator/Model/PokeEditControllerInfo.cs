using PokeSharp.Editor.Core;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.Editor.SourceGenerator.Model;

[AttributeInfoType<PokeEditControllerAttribute>]
public readonly record struct PokeEditControllerInfo(string? Name);
