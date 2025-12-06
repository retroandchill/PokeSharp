using PokeSharp.Core.Strings;
using PokeSharp.Unreal.Core.Interop;
using UnrealSharp.Core;
using UnrealSharp.Core.Marshallers;

namespace PokeSharp.Unreal.Core.Strings;

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

    public UnrealTextData(FTextData textData)
    {
        _textData = textData;
    }

    ~UnrealTextData()
    {
        PokeSharpTextExporter.CallDestroy(ref _textData);
    }

    public static UnrealTextData FromLocText(ReadOnlySpan<char> locString)
    {
        fixed (char* locStringPtr = locString)
        {
            PokeSharpTextExporter.CallFromLocText((IntPtr)locStringPtr, locString.Length, out var textData);
            return new UnrealTextData(textData);
        }
    }

    public FText ToUnrealText()
    {
        fixed (FTextData* textDataPtr = &_textData)
        {
            return TextMarshaller.FromNative((IntPtr)textDataPtr, 0);
        }
    }

    public ReadOnlySpan<char> AsDisplaySpan()
    {
        PokeSharpTextExporter.CallAsDisplaySpan(ref _textData, out var buffer, out var length);
        return new ReadOnlySpan<char>((char*)buffer, length);
    }

    public string ToLocString()
    {
        PokeSharpTextExporter.CallToLocText(ref _textData, out var buffer);
        var bufferPtr = &buffer;
        try
        {
            return StringMarshaller.FromNative((IntPtr)bufferPtr, 0);
        }
        finally
        {
            StringMarshaller.DestructInstance((IntPtr)bufferPtr, 0);
        }
    }
}

public static class UnrealTextDataExtensions
{
    extension(ITextData)
    {
        public static unsafe ITextData FromUnrealText(FText text)
        {
            var textData = new FTextData();
            var textDataPtr = (IntPtr)(&textData);
            TextMarshaller.ToNative(textDataPtr, 0, text);
            return new UnrealTextData(textData);
        }
    }
}
