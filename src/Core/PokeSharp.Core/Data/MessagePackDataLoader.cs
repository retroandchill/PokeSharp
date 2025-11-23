using System.Runtime.CompilerServices;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Options;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Core.Data;

/// <summary>
/// A data loader that facilitates saving and loading entities using MessagePack serialization.
/// </summary>
[RegisterSingleton]
public partial class MessagePackDataLoader(MessagePackSerializerOptions options) : IDataLoader
{
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
        await MessagePackSerializer.SerializeAsync(fileStream, entities, options, cancellationToken);
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
                options,
                cancellationToken
            )
        )
        {
            yield return entity;
        }
    }
}
