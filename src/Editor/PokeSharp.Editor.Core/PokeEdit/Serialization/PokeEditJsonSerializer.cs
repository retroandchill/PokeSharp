using System.Text.Json;
using Injectio.Attributes;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Editor.Core.PokeEdit.Serialization;

[RegisterSingleton]
public sealed partial class PokeEditJsonSerializer : IPokeEditSerializer
{
    [CreateSyncVersion]
    public async ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        return await JsonSerializer.DeserializeAsync<T>(
            stream,
            PokeEditJsonSerializerContext.Default.Options,
            cancellationToken
        );
    }

    [CreateSyncVersion]
    public async ValueTask SerializeAsync<T>(Stream stream, T? value, CancellationToken cancellationToken = default)
    {
        await JsonSerializer.SerializeAsync(
            stream,
            value,
            PokeEditJsonSerializerContext.Default.Options,
            cancellationToken
        );
    }
}
