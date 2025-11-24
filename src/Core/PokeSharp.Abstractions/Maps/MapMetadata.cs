using PokeSharp.Core;
using PokeSharp.Core.Strings;

namespace PokeSharp.Maps;

public interface IMapMetadata
{
    int Id { get; }

    Text Name { get; }

    int ParentId { get; }

    int Order { get; }

    bool Expanded { get; }
}
