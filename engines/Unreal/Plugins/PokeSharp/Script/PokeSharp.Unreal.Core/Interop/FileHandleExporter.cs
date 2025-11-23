using UnrealSharp.Binds;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Core.Interop;

[NativeCallbacks]
public static unsafe partial class FileHandleExporter
{
    private static readonly delegate* unmanaged<IntPtr, long> GetSize;
    private static readonly delegate* unmanaged<IntPtr, long> GetPosition;
    private static readonly delegate* unmanaged<IntPtr, NativeBool> Flush;
    private static readonly delegate* unmanaged<IntPtr, IntPtr, int, out int, NativeBool> Read;
    private static readonly delegate* unmanaged<IntPtr, long, SeekOrigin, out long, NativeBool> Seek;
    private static readonly delegate* unmanaged<IntPtr, long, NativeBool> SetLength;
    private static readonly delegate* unmanaged<IntPtr, IntPtr, int, NativeBool> Write;
    private static readonly delegate* unmanaged<IntPtr, void> Close;
}
