using System.Text.Json;
using Injectio.Attributes;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Editor.Core.PokeEdit.Serialization;

[RegisterSingleton]
public sealed partial class PokeEditJsonSerializer(JsonSerializerOptions options) : IPokeEditSerializer
{
    [CreateSyncVersion]
    public async ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        return await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
    }

    [CreateSyncVersion]
    public async ValueTask SerializeAsync<T>(Stream stream, T? value, CancellationToken cancellationToken = default)
    {
        await JsonSerializer.SerializeAsync(stream, value, options, cancellationToken);
    }
}
