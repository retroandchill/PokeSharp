using PokeSharp.Core.Data;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.Core.SourceGenerator.Model;

[AttributeInfoType<GameDataEntityAttribute>]
public readonly record struct GameDataEntityInfo
{
    public string? DataPath { get; init; }

    public bool IsOptional { get; init; }
}
