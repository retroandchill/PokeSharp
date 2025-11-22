using PokeSharp.Core.Unreal.Interop;
using UnrealSharp.Core;
using UnrealSharp.Core.Marshallers;

namespace PokeSharp.Core.Unreal;

internal sealed unsafe class UnrealTextData : ITextData
{
    private FTextData _textData;

    public string? SourceString
    {
        get
        {
            var nativeString = PokeSharpTextExporter.CallGetSourceString(ref _textData);
            return nativeString != IntPtr.Zero ? StringMarshaller.FromNative(nativeString, 0) : null;
        }
    }

    public string DisplayString
    {
        get
        {
            var nativeString = PokeSharpTextExporter.CallGetSourceString(ref _textData);
            return StringMarshaller.FromNative(nativeString, 0);
        }
    }

    public string? Namespace
    {
        get
        {
            var nativeString = new UnmanagedArray();
            var strPointer = (IntPtr)(&nativeString);
            if (PokeSharpTextExporter.CallGetNamespace(ref _textData, ref nativeString) == NativeBool.False)
                return null;

            try
            {
                return StringMarshaller.FromNative(strPointer, 0);
            }
            finally
            {
                StringMarshaller.DestructInstance(strPointer, 0);
            }
        }
    }

    public string? Key
    {
        get
        {
            var nativeString = new UnmanagedArray();
            var strPointer = (IntPtr)(&nativeString);
            if (PokeSharpTextExporter.CallGetKey(ref _textData, ref nativeString) == NativeBool.False)
                return null;

            try
            {
                return StringMarshaller.FromNative(strPointer, 0);
            }
            finally
            {
                StringMarshaller.DestructInstance(strPointer, 0);
            }
        }
    }

    public bool IsCultureInvariant => PokeSharpTextExporter.CallIsCultureInvariant(ref _textData).ToManagedBool();
    public bool IsTransient => PokeSharpTextExporter.CallIsTransient(ref _textData).ToManagedBool();
    public bool ShouldGatherForLocalization =>
        PokeSharpTextExporter.CallShouldGatherForLocalization(ref _textData).ToManagedBool();
    public bool IsEmpty => PokeSharpTextExporter.CallIsEmpty(ref _textData).ToManagedBool();
    public bool IsWhitespace => PokeSharpTextExporter.CallIsWhitespace(ref _textData).ToManagedBool();

    public UnrealTextData(ReadOnlySpan<char> sourceString)
    {
        fixed (char* sourceStringPtr = sourceString)
        {
            PokeSharpTextExporter.CallFromSourceString((IntPtr)sourceStringPtr, sourceString.Length, out _textData);
        }
    }

    public UnrealTextData(ReadOnlySpan<char> ns, ReadOnlySpan<char> key, ReadOnlySpan<char> sourceString)
    {
        fixed (char* nsStringPtr = ns)
        fixed (char* keyStringPtr = key)
        fixed (char* sourceStringPtr = sourceString)
        {
            PokeSharpTextExporter.CallFromLocalized(
                (IntPtr)nsStringPtr,
                ns.Length,
                (IntPtr)keyStringPtr,
                key.Length,
                (IntPtr)sourceStringPtr,
                sourceString.Length,
                out _textData
            );
        }
    }

    ~UnrealTextData()
    {
        PokeSharpTextExporter.CallDestroy(ref _textData);
    }

    public ReadOnlySpan<char> AsDisplaySpan()
    {
        PokeSharpTextExporter.CallAsDisplaySpan(ref _textData, out var buffer, out var length);
        return new ReadOnlySpan<char>((char*)buffer, length);
    }
}
