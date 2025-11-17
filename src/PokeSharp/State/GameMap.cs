using PokeSharp.Core;

namespace PokeSharp.State;

[AutoServiceShortcut]
public class GameMap
{
    public int MapId { get; }

    public bool HasMetadataTag(Name tagName) => false;

    public int RegionId { get; }
}
