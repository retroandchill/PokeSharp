using System.Runtime.Serialization;
using MessagePack;
using MessagePack.Formatters;
using PokeSharp.Core.Serialization.MessagePack;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Serialization.MessagePack;

public class EvolutionParameterFormatter : IMessagePackFormatter<>
{
    public void Serialize(ref MessagePackWriter writer, EvolutionParameter value, MessagePackSerializerOptions options)
    {
        writer.Write((byte)value.Kind);
        switch (value.Kind)
        {
            case EvolutionParameterType.Null:
                break;
            case EvolutionParameterType.Int:
                writer.Write(value.GetInteger());
                break;
            case EvolutionParameterType.Name:
                NameMessagePackFormatter.Default.Serialize(ref writer, value.GetName(), options);
                break;
            default:
                throw new SerializationException("Unknown EvolutionParameterType");
        }
    }

    public EvolutionParameter Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var kind = (EvolutionParameterType) reader.ReadByte();
        return kind switch
        {
            EvolutionParameterType.Null => EvolutionParameter.Null,
            EvolutionParameterType.Int => reader.ReadInt32(),
            EvolutionParameterType.Name => NameMessagePackFormatter.Default.Deserialize(ref reader, options),
            _ => throw new SerializationException("Invalid kind")
        };
    }
}