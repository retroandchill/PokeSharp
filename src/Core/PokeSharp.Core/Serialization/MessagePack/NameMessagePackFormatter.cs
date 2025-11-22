using MessagePack;
using MessagePack.Formatters;

namespace PokeSharp.Core.Serialization.MessagePack;

/// <summary>
/// Provides a MessagePack formatter for the <see cref="Name"/> struct.
/// </summary>
public class NameMessagePackFormatter : IMessagePackFormatter<Name>
{
    /// <inheritdoc />
    public void Serialize(ref MessagePackWriter writer, Name value, MessagePackSerializerOptions options)
    {
        writer.Write(value.ToString());
    }

    /// <inheritdoc />
    public Name Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var readString = reader.ReadString();
        return !string.IsNullOrEmpty(readString) ? readString : Name.None;
    }
}
