using UnrealSharp.Binds;

namespace PokeSharp.Unreal.Core.Interop;

[NativeCallbacks]
public static unsafe partial class SettingsChangeExporter
{
    private static readonly delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> RegisterSettingsChangeCallback;
    private static readonly delegate* unmanaged<IntPtr, void> UnregisterSettingsChangeCallback;
}
