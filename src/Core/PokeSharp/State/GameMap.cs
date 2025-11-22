using Injectio.Attributes;
using PokeSharp.Core;

namespace PokeSharp.State;

[RegisterSingleton]
[AutoServiceShortcut]
public class GameMap
{
    public int MapId { get; }

    public bool HasMetadataTag(Name tagName) => false;

    public int RegionId { get; }
}
