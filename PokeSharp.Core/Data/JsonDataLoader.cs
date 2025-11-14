using System.Runtime.CompilerServices;
using System.Text.Json;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Core.Data;

/// <summary>
/// Represents a data loader implementation for saving and loading JSON-serialized entities.
/// </summary>
/// <remarks>
/// This class is registered as a singleton with a JSON serializer tag and provides
/// methods for saving and loading entities in JSON format. It is intended to be a specialized
/// implementation that handles serialization and deserialization using System.Text.Json.
/// </remarks>
[RegisterSingleton(Tags = SerializerTags.Json)]
public partial class JsonDataLoader : IDataLoader
{
    /// <inheritdoc />
    [CreateSyncVersion]
    public async ValueTask SaveEntitiesAsync<T>(
        IEnumerable<T> entities,
        string outputPath,
        CancellationToken cancellationToken = default
    )
    {
        await using var fileStream = File.OpenWrite($"{outputPath}.json");
        await JsonSerializer.SerializeAsync(fileStream, entities, JsonSerializerOptions.Default, cancellationToken);
    }

    /// <inheritdoc />
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
    }
}
