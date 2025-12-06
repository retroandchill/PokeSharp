using UnrealSharp.Binds;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Core.Interop;

[NativeCallbacks]
public static unsafe partial class PokeSharpTextExporter
{
    private static readonly delegate* unmanaged<IntPtr, int, out FTextData, void> FromSourceString;
    private static readonly delegate* unmanaged<
        IntPtr,
        int,
        IntPtr,
        int,
        IntPtr,
        int,
        out FTextData,
        void> FromLocalized;
    private static readonly delegate* unmanaged<IntPtr, int, out FTextData, void> FromLocText;
    private static readonly delegate* unmanaged<ref FTextData, out UnmanagedArray, void> ToLocText;
    private static readonly delegate* unmanaged<ref FTextData, IntPtr> GetSourceString;
    private static readonly delegate* unmanaged<ref FTextData, IntPtr> GetDisplayString;
    private static readonly delegate* unmanaged<ref FTextData, ref UnmanagedArray, NativeBool> GetNamespace;
    private static readonly delegate* unmanaged<ref FTextData, ref UnmanagedArray, NativeBool> GetKey;
    private static readonly delegate* unmanaged<ref FTextData, NativeBool> IsCultureInvariant;
    private static readonly delegate* unmanaged<ref FTextData, NativeBool> IsTransient;
    private static readonly delegate* unmanaged<ref FTextData, NativeBool> ShouldGatherForLocalization;
    private static readonly delegate* unmanaged<ref FTextData, NativeBool> IsEmpty;
    private static readonly delegate* unmanaged<ref FTextData, NativeBool> IsWhitespace;
    private static readonly delegate* unmanaged<ref FTextData, void> Destroy;
    private static readonly delegate* unmanaged<ref FTextData, out IntPtr, out int, void> AsDisplaySpan;
}
