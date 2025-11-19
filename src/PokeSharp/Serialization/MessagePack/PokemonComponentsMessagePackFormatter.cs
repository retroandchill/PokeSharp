using MessagePack;
using MessagePack.Formatters;
using PokeSharp.Core;
using PokeSharp.PokemonModel;

namespace PokeSharp.Serialization.MessagePack;

public class PokemonComponentsMessagePackFormatter : IMessagePackFormatter<Dictionary<Name, IPokemonComponent>?>
{
    public void Serialize(ref MessagePackWriter writer, Dictionary<Name, IPokemonComponent>? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteMapHeader(value.Count);
        foreach (var (key, component) in value)
        {
            writer.Write(key);
            TypelessFormatter.Instance.Serialize(ref writer, component, options);
        }
    }

    public Dictionary<Name, IPokemonComponent>? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }
        
        var result = new Dictionary<Name, IPokemonComponent>();
        var mapSize = reader.ReadMapHeader();
        for (var i = 0; i < mapSize; i++)
        {
            Name key = reader.ReadString()!;
            var component = (IPokemonComponent)TypelessFormatter.Instance.Deserialize(ref reader, options)!;
            result.Add(key, component);
        }

        return result;
    }
}