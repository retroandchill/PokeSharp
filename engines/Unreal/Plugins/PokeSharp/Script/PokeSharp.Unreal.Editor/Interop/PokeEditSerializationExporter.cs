using UnrealSharp.Binds;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Editor.Interop;

[NativeCallbacks]
public static unsafe partial class PokeEditSerializationExporter
{
    private static readonly delegate* unmanaged<IntPtr, int, ref UnmanagedArray, void> SerializeString;
    private static readonly delegate* unmanaged<IntPtr, int, ref UnmanagedArray, void> SerializeByteArray;
}
