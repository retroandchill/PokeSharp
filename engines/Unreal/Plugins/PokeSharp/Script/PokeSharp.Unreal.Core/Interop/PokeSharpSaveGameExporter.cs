using UnrealSharp.Binds;

namespace PokeSharp.Unreal.Core.Interop;

[NativeCallbacks]
public static unsafe partial class PokeSharpSaveGameExporter
{
    private static readonly delegate* unmanaged<IntPtr, out IntPtr, out int, void> GetDataReadBuffer;
    private static readonly delegate* unmanaged<IntPtr, IntPtr, int, void> WriteToDataBuffer;
}
