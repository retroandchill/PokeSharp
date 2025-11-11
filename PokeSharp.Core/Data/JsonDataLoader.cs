using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Core.Data;

public partial class JsonDataLoader : IDataLoader
{
    [CreateSyncVersion]
    public async ValueTask SaveEntitiesAsync<T>(
        IEnumerable<T> entities,
        string outputPath,
        CancellationToken cancellationToken = default
    )
    {
        await using var fileStream = File.OpenWrite($"{outputPath}.json");
        await JsonSerializer.SerializeAsync(
            fileStream,
            entities,
            JsonSerializerOptions.Default,
            cancellationToken
        );
    }

    [CreateSyncVersion]
    public async IAsyncEnumerable<T> LoadEntitiesAsync<T>(
        string inputPath,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await using var fileStream = File.OpenRead($"{inputPath}.json");
        var deserializedValue = await JsonSerializer.DeserializeAsync<IEnumerable<T>>(
            fileStream,
            JsonSerializerOptions.Default,
            cancellationToken
        );
        ArgumentNullException.ThrowIfNull(deserializedValue);

        foreach (var entity in deserializedValue)
        {
            yield return entity;
        }
        ;
    }
}
