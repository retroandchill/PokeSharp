using System.Text.Json;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Serialization;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IRequestParameterReader
{
    int ParameterIndex { get; }

    bool ReadBoolean();
    byte ReadByte();
    int ReadInt32();
    long ReadInt64();
    float ReadSingle();
    double ReadDouble();
    Guid ReadGuid();
    Name ReadName();

    ReadOnlySpan<char> ReadString();
    ReadOnlySpan<byte> ReadBytes();

    T ReadEnum<T>()
        where T : unmanaged, Enum;
    T? ReadSerialized<T>(IPokeEditSerializer serializer);

    void Skip();
}
