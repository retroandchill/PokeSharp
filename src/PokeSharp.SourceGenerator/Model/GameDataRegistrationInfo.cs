using Microsoft.CodeAnalysis;
using PokeSharp.SourceGenerator.Attributes;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.SourceGenerator.Model;

[AttributeInfoType(typeof(GameDataRegistrationAttribute<>))]
public readonly record struct GameDataRegistrationInfo(ITypeSymbol Type, int Priority);
