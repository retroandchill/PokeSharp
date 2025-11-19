using MessagePack;
using MessagePack.Formatters;

namespace PokeSharp.Serialization.MessagePack;

public class EvolutionInfoParameterFormatter : IMessagePackFormatter<object?>
{
    public void Serialize(ref MessagePackWriter writer, object? value, MessagePackSerializerOptions options)
    {
        TypelessFormatter.Instance.Serialize(ref writer, value, options);
    }

    public object? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        return TypelessFormatter.Instance.Deserialize(ref reader, options);
    }
}