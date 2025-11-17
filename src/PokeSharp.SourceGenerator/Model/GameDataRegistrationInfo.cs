using Microsoft.CodeAnalysis;
using PokeSharp.Core.Data;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.SourceGenerator.Model;

[AttributeInfoType(typeof(GameDataRegistrationAttribute<>))]
public readonly record struct GameDataRegistrationInfo(ITypeSymbol Type, int Priority);
