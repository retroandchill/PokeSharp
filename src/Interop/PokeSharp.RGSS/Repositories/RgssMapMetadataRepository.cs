using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using PokeSharp.Maps;
using PokeSharp.RGSS.RPG;
using PokeSharp.RGSS.Serialization;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.RGSS.Repositories;

[RegisterSingleton]
public partial class RgssMapMetadataRepository : IMapMetadataRepository
{
    private readonly Dictionary<int, RgssMapMetadata> _entries = new();

    public int Count => _entries.Count;
    public IEnumerable<IMapMetadata> Entries => _entries.Values;

    public bool Exists(int id) => _entries.ContainsKey(id);

    public IMapMetadata Get(int id) => _entries[id];

    public bool TryGet(int id, [NotNullWhen(true)] out IMapMetadata? metadata)
    {
        if (_entries.TryGetValue(id, out var mapData))
        {
            metadata = mapData;
            return true;
        }

        metadata = null;
        return false;
    }

    [CreateSyncVersion]
    public async ValueTask LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var streamReader = File.OpenRead("Data/MapInfos.json");
        var foundMapInfos = await JsonSerializer.DeserializeAsync(
            streamReader,
            RgssJsonSerializerContext.Default.DictionaryInt32MapInfo,
            cancellationToken
        );
        ArgumentNullException.ThrowIfNull(foundMapInfos);

        _entries.Clear();
        foreach (var (id, mapInfo) in foundMapInfos)
        {
            _entries.Add(id, new RgssMapMetadata(id, mapInfo));
        }
    }
}
