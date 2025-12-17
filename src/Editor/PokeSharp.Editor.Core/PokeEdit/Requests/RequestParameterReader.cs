using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IRequestParameterReader
{
    int? ParameterCount { get; }
    
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
    
    T ReadEnum<T>() where T : unmanaged, Enum;
    T ReadSerialized<T>();

    void Skip();
}