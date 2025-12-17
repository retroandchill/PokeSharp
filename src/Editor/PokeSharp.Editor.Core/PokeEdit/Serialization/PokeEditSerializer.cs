namespace PokeSharp.Editor.Core.PokeEdit.Serialization;

public interface IPokeEditSerializer
{
    T? Deserialize<T>(ReadOnlySpan<byte> buffer);

    byte[] Serialize<T>(T? value);
}
