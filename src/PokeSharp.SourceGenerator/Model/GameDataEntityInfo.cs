using PokeSharp.Core.Data;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.SourceGenerator.Model;

[AttributeInfoType<GameDataEntityAttribute>]
public readonly record struct GameDataEntityInfo
{
    public string? DataPath { get; init; }
}
