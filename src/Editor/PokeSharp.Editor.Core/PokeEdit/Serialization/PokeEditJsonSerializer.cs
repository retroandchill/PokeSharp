using System.Text.Json;
using Injectio.Attributes;

namespace PokeSharp.Editor.Core.PokeEdit.Serialization;

[RegisterSingleton]
public sealed class PokeEditJsonSerializer(JsonSerializerOptions options) : IPokeEditSerializer
{
    public T? Deserialize<T>(ReadOnlySpan<byte> buffer)
    {
        return JsonSerializer.Deserialize<T>(buffer, options);
    }

    public byte[] Serialize<T>(T? value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, options);
    }
}
