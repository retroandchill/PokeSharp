using PokeSharp.Core;
using PokeSharp.Maps;
using PokeSharp.RGSS.RPG;
using Retro.ReadOnlyParams.Annotations;

namespace PokeSharp.RGSS.Repositories;

public class RgssMapMetadata(int id, [ReadOnly] MapInfo mapInfo) : IMapMetadata
{
    public int Id { get; } = id;

    public Text Name => mapInfo.Name;
    public int ParentId => mapInfo.ParentId;
    public int Order => mapInfo.Order;
    public bool Expanded => mapInfo.Expanded;
}
