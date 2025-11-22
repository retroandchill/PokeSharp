using UnrealSharp.Binds;
using UnrealSharp.Core;

namespace PokeSharp.Core.Unreal.Interop;

[NativeCallbacks]
public static unsafe partial class PokeSharpNameExporter
{
    private static readonly delegate* unmanaged<
        IntPtr,
        int,
        NativeBool,
        out uint,
        out uint,
        out int,
        void> GetOrAddEntry;
    private static readonly delegate* unmanaged<uint, uint, int, IntPtr, int, NativeBool> EqualsBuffer;
    private static readonly delegate* unmanaged<uint, uint, int, ref UnmanagedArray, void> GetString;
}
