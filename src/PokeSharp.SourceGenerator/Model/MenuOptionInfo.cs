using Microsoft.CodeAnalysis;
using PokeSharp.UI;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.SourceGenerator.Model;

[AttributeInfoType(typeof(MenuOptionRegistrationAttribute<>))]
public readonly record struct MenuOptionRegistrationInfo(ITypeSymbol Type, int Priority);

[AttributeInfoType<MenuOptionAttribute>]
public readonly record struct MenuOptionInfo(string? Name);
