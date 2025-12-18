using System.Text.Json;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Serialization;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IResponseWriter
{
    void WriteBoolean(bool value);
    void WriteByte(byte value);
    void WriteInt32(int value);
    void WriteInt64(long value);
    void WriteSingle(float value);
    void WriteDouble(double value);
    void WriteGuid(Guid value);
    void WriteName(Name value);

    void WriteString(ReadOnlySpan<char> value);
    void WriteBytes(ReadOnlySpan<byte> value);

    void WriteEnum<T>(T value)
        where T : unmanaged, Enum;
    void WriteSerialized<T>(T value, IPokeEditSerializer serializer);
}
