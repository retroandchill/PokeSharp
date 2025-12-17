using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Requests;
using PokeSharp.Editor.Core.PokeEdit.Serialization;
using PokeSharp.Unreal.Core.Strings;
using UnrealSharp.Core;
using UnrealSharp.Core.Marshallers;
using UnrealSharp.CoreUObject;

namespace PokeSharp.Unreal.Editor.PokeEdit.Requests;

public ref struct UnrealRequestParameterReader(IntPtr buffer, ReadOnlySpan<IntPtr> offsets) : IRequestParameterReader
{
    public int ParameterIndex { get; private set; }
    
    private readonly ReadOnlySpan<IntPtr> _offsets  = offsets;

    private IntPtr CurrentPosition
    {
        get
        {
            if (ParameterIndex == _offsets.Length) throw new InvalidOperationException("No more parameters to read");

            return buffer + _offsets[ParameterIndex];
        }
    }
    
    public bool ReadBoolean()
    {
        var value = BoolMarshaller.FromNative(CurrentPosition, 0);
        ParameterIndex++;
        return value;
    }

    public byte ReadByte()
    {
        var value = BlittableMarshaller<byte>.FromNative(CurrentPosition, 0);
        ParameterIndex++;
        return value;
    }

    public int ReadInt32()
    {
        var value = BlittableMarshaller<int>.FromNative(CurrentPosition, 0);
        ParameterIndex++;
        return value;
    }

    public long ReadInt64()
    {
        var value = BlittableMarshaller<long>.FromNative(CurrentPosition, 0);
        ParameterIndex++;
        return value;
    }

    public float ReadSingle()
    {
        var value = BlittableMarshaller<float>.FromNative(CurrentPosition, 0);
        ParameterIndex++;
        return value;
    }

    public double ReadDouble()
    {
        var value = BlittableMarshaller<double>.FromNative(CurrentPosition, 0);
        ParameterIndex++;
        return value;
    }

    public Guid ReadGuid()
    {
        var value = FGuid.FromNative(CurrentPosition);
        ParameterIndex++;
        return value;
    }

    public Name ReadName()
    {
        var value = BlittableMarshaller<FName>.FromNative(CurrentPosition, 0);
        ParameterIndex++;
        return value.ToPokeSharpName();
    }

    public ReadOnlySpan<char> ReadString()
    {
        unsafe
        {
            var value = *(UnmanagedArray*)CurrentPosition;
            ParameterIndex++;
            return new ReadOnlySpan<char>((char*)value.Data, value.ArrayNum);
        }
    }

    public ReadOnlySpan<byte> ReadBytes()
    {
        unsafe
        {
            var value = *(UnmanagedArray*)CurrentPosition;
            ParameterIndex++;
            return new ReadOnlySpan<byte>((byte*)value.Data, value.ArrayNum);
        }
    }

    public T ReadEnum<T>() where T : unmanaged, Enum
    {
        var value = BlittableMarshaller<T>.FromNative(CurrentPosition, 0);
        ParameterIndex++;
        return value;
    }

    public T? ReadSerialized<T>(IPokeEditSerializer serializer)
    {
        var bytes = ReadBytes();
        return serializer.Deserialize<T>(bytes);
    }

    public void Skip()
    {
        ParameterIndex++;
    }
}