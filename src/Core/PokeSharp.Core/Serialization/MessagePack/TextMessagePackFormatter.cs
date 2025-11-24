using MessagePack;
using MessagePack.Formatters;
using PokeSharp.Core.Strings;

namespace PokeSharp.Core.Serialization.MessagePack;

/// <summary>
/// A custom MessagePack formatter for the <see cref="Text"/> struct, enabling serialization
/// and deserialization of the <see cref="Text"/> object for MessagePack integration.
/// </summary>
/// <remarks>
/// This formatter is used to convert <see cref="Text"/> to and from MessagePack's binary format,
/// ensuring compatibility and efficient data storage. It primarily handles the conversion of
/// <see cref="Text"/> into its string representation for serialization and reconstructs a
/// <see cref="Text"/> object from a string during deserialization.
/// </remarks>
/// <threadsafety>
/// This class is stateless, and its methods are thread-safe as long as the underlying
/// MessagePack runtime resources are safely used.
/// </threadsafety>
/// <seealso cref="Text"/>
/// <seealso cref="IMessagePackFormatter{T}"/>
public class TextMessagePackFormatter : IMessagePackFormatter<Text>
{
    /// <inheritdoc />
    public void Serialize(ref MessagePackWriter writer, Text value, MessagePackSerializerOptions options)
    {
        writer.Write(value.ToString());
    }

    /// <inheritdoc />
    public Text Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var readString = reader.ReadString();
        return !string.IsNullOrEmpty(readString) ? readString : Text.None;
    }
}
