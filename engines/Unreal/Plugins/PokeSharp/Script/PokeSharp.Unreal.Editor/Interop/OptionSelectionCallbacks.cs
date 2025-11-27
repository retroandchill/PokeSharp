using System.Runtime.InteropServices;
using JetBrains.Annotations;
using PokeSharp.Editor.Data;
using PokeSharp.Unreal.Core.Strings;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Editor.Interop;

[StructLayout(LayoutKind.Sequential)]
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
public unsafe struct OptionSelectionCallbacks
{
    public required delegate* unmanaged<FName, UnmanagedArray*, void> GetNamesList { get; init; }

    public static OptionSelectionCallbacks Create()
    {
        return new OptionSelectionCallbacks { GetNamesList = &OptionSelectionMethods.GetNamesList };
    }
}

internal static unsafe class OptionSelectionMethods
{
    [UnmanagedCallersOnly]
    public static void GetNamesList(FName self, UnmanagedArray* namesList)
    {
        var (count, names) = DataOptionsManager.GetNameOptions(self.ToPokeSharpName());
        OptionSelectionExporter.CallSetArraySize(ref *namesList, count);
        var optionsSpan = new Span<FName>((FName*)namesList->Data, count);
        foreach (var (i, name) in names.Index())
        {
            optionsSpan[i] = name.ToUnrealName();
        }
    }
}
