using MessagePack;
using MessagePack.Formatters;
using PokeSharp.Core.Strings;

namespace PokeSharp.Core.Serialization.MessagePack;

/// <summary>
/// Provides a MessagePack formatter for the <see cref="Name"/> struct.
/// </summary>
public abstract class NameMessagePackFormatterBase<T> : IMessagePackFormatter<T>
    where T : struct, IName<T>
{
    /// <inheritdoc />
    public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
    {
        writer.Write(value.ToString());
    }

    /// <inheritdoc />
    public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var readString = reader.ReadString();
        return !string.IsNullOrEmpty(readString) ? T.FromString(readString) : T.None;
    }
}

public class NameMessagePackFormatter : NameMessagePackFormatterBase<Name>;

public class CaselessNameMessagePackFormatter : NameMessagePackFormatterBase<CaselessName>;
