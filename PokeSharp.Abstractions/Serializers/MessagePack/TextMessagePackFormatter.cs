using MessagePack;
using MessagePack.Formatters;

namespace PokeSharp.Abstractions.Serializers.MessagePack;

[MessagePackFormatter(typeof(TextMessagePackFormatter))]
public class TextMessagePackFormatter : IMessagePackFormatter<Text>
{
    public void Serialize(
        ref MessagePackWriter writer,
        Text value,
        MessagePackSerializerOptions options
    )
    {
        writer.Write(value.ToString());
    }

    public Text Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var readString = reader.ReadString();
        return !string.IsNullOrEmpty(readString) ? readString : Text.None;
    }
}
