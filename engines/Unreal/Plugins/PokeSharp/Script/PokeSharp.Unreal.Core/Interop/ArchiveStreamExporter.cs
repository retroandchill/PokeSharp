using PokeSharp.Unreal.Core.Utils;
using UnrealSharp.Binds;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Core.Interop;

[NativeCallbacks]
public static unsafe partial class ArchiveStreamExporter
{
    private static readonly delegate* unmanaged<ref SharedPtr, ref SharedPtr, void> Copy;
    private static readonly delegate* unmanaged<ref SharedPtr, void> Release;
    private static readonly delegate* unmanaged<IntPtr, NativeBool> CanRead;
    private static readonly delegate* unmanaged<IntPtr, NativeBool> CanWrite;
    private static readonly delegate* unmanaged<IntPtr, NativeBool> CanSeek;
    private static readonly delegate* unmanaged<IntPtr, long> GetLength;
    private static readonly delegate* unmanaged<IntPtr, long> GetPosition;
    private static readonly delegate* unmanaged<IntPtr, long, NativeBool> SetPosition;
    private static readonly delegate* unmanaged<IntPtr, void> Flush;
    private static readonly delegate* unmanaged<IntPtr, IntPtr, long, out int, NativeBool> Read;
    private static readonly delegate* unmanaged<IntPtr, long, SeekOrigin, out long, NativeBool> Seek;
    private static readonly delegate* unmanaged<IntPtr, IntPtr, int, NativeBool> Write;
}
