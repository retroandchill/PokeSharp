using PokeSharp.Abstractions;

namespace PokeSharp.Core.State;

public class GameMap
{
    public static GameMap Instance { get; } = new();

    public Name MapId { get; }

    public bool HasMetadataTag(Name tagName) => false;

    public int RegionId { get; }
}
