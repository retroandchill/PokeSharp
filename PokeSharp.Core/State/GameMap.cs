using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Core.State;

[AutoServiceShortcut]
public class GameMap
{
    public Name MapId { get; }

    public bool HasMetadataTag(Name tagName) => false;

    public int RegionId { get; }
}
