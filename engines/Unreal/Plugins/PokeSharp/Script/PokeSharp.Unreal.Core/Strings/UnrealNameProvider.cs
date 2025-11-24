using PokeSharp.Core.Strings;
using PokeSharp.Unreal.Core.Interop;
using UnrealSharp.Core;
using UnrealSharp.Core.Marshallers;

namespace PokeSharp.Unreal.Core.Strings;

internal unsafe class UnrealNameProvider : INameProvider
{
    public (uint ComparisonIndex, uint DisplayIndex, int Number) GetOrAddEntry(
        ReadOnlySpan<char> value,
        FindName findType
    )
    {
        fixed (char* valuePtr = value)
        {
            PokeSharpNameExporter.CallGetOrAddEntry(
                (IntPtr)valuePtr,
                value.Length,
                findType == FindName.Find ? NativeBool.True : NativeBool.False,
                out var comparisonIndex,
                out var displayIndex,
                out var number
            );
            return (comparisonIndex, displayIndex, number);
        }
    }

    public bool IsValid(uint comparisonIndex, uint displayIndex)
    {
        return PokeSharpNameExporter.CallIsValid(comparisonIndex, displayIndex).ToManagedBool();
    }

    public bool Equals(uint comparisonIndex, uint displayIndex, int number, ReadOnlySpan<char> span)
    {
        fixed (char* spanPtr = span)
        {
            return PokeSharpNameExporter
                .CallEqualsBuffer(comparisonIndex, displayIndex, number, (IntPtr)spanPtr, span.Length)
                .ToManagedBool();
        }
    }

    public string GetString(uint comparisonIndex, uint displayStringId, int number)
    {
        var result = new UnmanagedArray();
        var strPointer = (IntPtr)(&result);
        try
        {
            PokeSharpNameExporter.CallGetString(comparisonIndex, displayStringId, number, ref result);
            return StringMarshaller.FromNative(strPointer, 0);
        }
        finally
        {
            StringMarshaller.DestructInstance(strPointer, 0);
        }
    }
}
