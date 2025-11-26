using UnrealSharp.Binds;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Editor.Interop;

[NativeCallbacks]
public static unsafe partial class OptionSelectionExporter
{
    private static readonly delegate* unmanaged<ref UnmanagedArray, int, void> SetArraySize;
}
