using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Strings;

namespace PokeSharp.State;

[RegisterSingleton]
[AutoServiceShortcut]
public class GameMap
{
    public int MapId { get; }

    public bool HasMetadataTag(Name tagName) => false;

    public int RegionId { get; }
}
