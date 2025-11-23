using UnrealSharp.Binds;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Core.Interop;

[NativeCallbacks]
public static unsafe partial class PlatformFileExporter
{
    private static readonly delegate* unmanaged<IntPtr, NativeBool, IntPtr> OpenRead;
    private static readonly delegate* unmanaged<IntPtr, NativeBool, NativeBool, NativeBool, IntPtr> OpenWrite;
}
