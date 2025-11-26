using UnrealSharp.Binds;

namespace PokeSharp.Unreal.Editor.Interop;

[NativeCallbacks]
public static unsafe partial class PokeSharpEditorCallbacksExporter
{
    private static readonly delegate* unmanaged<OptionSelectionCallbacks, void> SetOptionSelectionCallbacks;
}
