namespace PokeSharp.Editor.Core.PokeEdit.Serialization;

public interface IPokeEditSerializer
{
    T? Deserialize<T>(Stream stream);

    ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default);

    void Serialize<T>(Stream stream, T? value);

    ValueTask SerializeAsync<T>(Stream stream, T? value, CancellationToken cancellationToken = default);
}
