using UnrealSharp.Binds;

namespace PokeSharp.Unreal.Editor.Interop;

[NativeCallbacks]
public static unsafe partial class PokeSharpEditorCallbacksExporter
{
    private static readonly delegate* unmanaged<OptionSelectionCallbacks, void> SetOptionSelectionCallbacks;
    private static readonly delegate* unmanaged<PokeEditCallbacks, void> SetPokeEditCallbacks;
}
