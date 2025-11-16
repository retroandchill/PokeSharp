using System.Runtime.CompilerServices;
using MessagePack;
using MessagePack.Resolvers;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Core.Data;

/// <summary>
/// A data loader that facilitates saving and loading entities using MessagePack serialization.
/// </summary>
[RegisterSingleton(Tags = SerializerTags.MessagePack)]
public partial class MessagePackDataLoader : IDataLoader
{
    private readonly MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard.WithResolver(
        ContractlessStandardResolver.Instance
    );

    /// <inheritdoc />
    [CreateSyncVersion]
    public async ValueTask SaveEntitiesAsync<T>(
        IEnumerable<T> entities,
        string outputPath,
        CancellationToken cancellationToken = default
    )
    {
        if (!Directory.Exists("Data"))
        {
            Directory.CreateDirectory("Data");
        }
        
        await using var fileStream = File.OpenWrite(Path.Join("Data", $"{outputPath}.pkdata"));
        await MessagePackSerializer.SerializeAsync(fileStream, entities, _options, cancellationToken);
    }

    /// <inheritdoc />
    [CreateSyncVersion]
    public async IAsyncEnumerable<T> LoadEntitiesAsync<T>(
        string inputPath,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await using var fileStream = File.OpenRead(Path.Join("Data", $"{inputPath}.pkdata"));
        foreach (
            var entity in await MessagePackSerializer.DeserializeAsync<IEnumerable<T>>(
                fileStream,
                _options,
                cancellationToken
            )
        )
        {
            yield return entity;
        }
    }
}
