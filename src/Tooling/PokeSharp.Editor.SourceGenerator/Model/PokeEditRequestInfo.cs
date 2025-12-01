using PokeSharp.Editor.Core;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.Editor.SourceGenerator.Model;

[AttributeInfoType<PokeEditRequestAttribute>]
public readonly record struct PokeEditRequestInfo(string? Name);
