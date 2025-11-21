using MessagePack;
using MessagePack.Formatters;
using Semver;

namespace PokeSharp.Core.Serialization.MessagePack;

public class SemVersionFormatter : IMessagePackFormatter<SemVersion?>
{
    public static readonly SemVersionFormatter Instance = new();

    private SemVersionFormatter() { }

    public void Serialize(ref MessagePackWriter writer, SemVersion? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.Write(value.ToString());
    }

    public SemVersion? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        return !reader.TryReadNil() ? SemVersion.Parse(reader.ReadString()!) : null;
    }
}
