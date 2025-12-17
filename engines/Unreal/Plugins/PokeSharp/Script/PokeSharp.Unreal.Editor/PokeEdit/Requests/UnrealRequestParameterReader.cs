using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Requests;

namespace PokeSharp.Unreal.Editor.PokeEdit.Requests;

public ref struct UnrealRequestParameterReader(IntPtr buffer, ReadOnlySpan<IntPtr> offsets) : IRequestParameterReader
{
    public int ParameterIndex { get; }
    
    public bool ReadBoolean()
    {
        throw new NotImplementedException();
    }

    public byte ReadByte()
    {
        throw new NotImplementedException();
    }

    public int ReadInt32()
    {
        throw new NotImplementedException();
    }

    public long ReadInt64()
    {
        throw new NotImplementedException();
    }

    public float ReadSingle()
    {
        throw new NotImplementedException();
    }

    public double ReadDouble()
    {
        throw new NotImplementedException();
    }

    public Guid ReadGuid()
    {
        throw new NotImplementedException();
    }

    public Name ReadName()
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<char> ReadString()
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<byte> ReadBytes()
    {
        throw new NotImplementedException();
    }

    public T ReadEnum<T>() where T : unmanaged, Enum
    {
        throw new NotImplementedException();
    }

    public T ReadSerialized<T>()
    {
        throw new NotImplementedException();
    }

    public void Skip()
    {
        throw new NotImplementedException();
    }
}