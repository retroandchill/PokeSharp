using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Requests;
using PokeSharp.Editor.Core.PokeEdit.Serialization;
using PokeSharp.Unreal.Core.Strings;
using PokeSharp.Unreal.Editor.Interop;
using UnrealSharp.Core;
using UnrealSharp.Core.Marshallers;
using UnrealSharp.CoreUObject;

namespace PokeSharp.Unreal.Editor.PokeEdit.Requests;

public readonly ref struct UnrealResponseWriter(IntPtr buffer) : IResponseWriter
{
    public void WriteBoolean(bool value)
    {
        BoolMarshaller.ToNative(buffer, 0, value);
    }

    public void WriteByte(byte value)
    {
        BlittableMarshaller<byte>.ToNative(buffer, 0, value);
    }

    public void WriteInt32(int value)
    {
        BlittableMarshaller<int>.ToNative(buffer, 0, value);
    }

    public void WriteInt64(long value)
    {
        BlittableMarshaller<long>.ToNative(buffer, 0, value);
    }

    public void WriteSingle(float value)
    {
        BlittableMarshaller<float>.ToNative(buffer, 0, value);
    }

    public void WriteDouble(double value)
    {
        BlittableMarshaller<double>.ToNative(buffer, 0, value);
    }

    public void WriteGuid(Guid value)
    {
        var unrealGuid = (FGuid)value;
        unrealGuid.ToNative(buffer);
    }

    public void WriteName(Name value)
    {
        BlittableMarshaller<FName>.ToNative(buffer, 0, value.ToUnrealName());
    }

    public void WriteString(ReadOnlySpan<char> value)
    {
        unsafe
        {
            fixed (char* ptr = value)
            {
                PokeEditSerializationExporter.CallSerializeString(
                    (IntPtr)ptr,
                    value.Length,
                    ref *(UnmanagedArray*)buffer
                );
            }
        }
    }

    public void WriteBytes(ReadOnlySpan<byte> value)
    {
        unsafe
        {
            fixed (byte* ptr = value)
            {
                PokeEditSerializationExporter.CallSerializeByteArray(
                    (IntPtr)ptr,
                    value.Length,
                    ref *(UnmanagedArray*)buffer
                );
            }
        }
    }

    public void WriteEnum<T>(T value)
        where T : unmanaged, Enum
    {
        BlittableMarshaller<T>.ToNative(buffer, 0, value);
    }

    public void WriteSerialized<T>(T? value, IPokeEditSerializer serializer)
    {
        var bytes = serializer.Serialize(value);
        WriteBytes(bytes);
    }
}
