using System.Diagnostics.CodeAnalysis;
using PokeSharp.Core;
using PokeSharp.Core.Data;

namespace PokeSharp.Maps;

[AutoServiceShortcut]
public interface IMapMetadataRepository : IDataRepository
{
    public int Count { get; }

    public IEnumerable<IMapMetadata> Entries { get; }

    public bool Exists(int id);

    public IMapMetadata Get(int id);

    public bool TryGet(int id, [NotNullWhen(true)] out IMapMetadata? metadata);
}
