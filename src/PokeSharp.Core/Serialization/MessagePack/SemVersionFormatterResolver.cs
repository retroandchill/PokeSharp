using MessagePack;
using MessagePack.Formatters;
using Semver;

namespace PokeSharp.Core.Serialization.MessagePack;

[RegisterSingleton]
public class SemVersionFormatterResolver : IFormatterResolver
{
    public IMessagePackFormatter<T>? GetFormatter<T>()
    {
        if (typeof(T) == typeof(SemVersion))
        {
            return (IMessagePackFormatter<T>)SemVersionFormatter.Instance;
        }

        return null;
    }
}
