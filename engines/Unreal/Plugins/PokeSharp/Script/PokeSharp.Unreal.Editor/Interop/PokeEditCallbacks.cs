using System.Runtime.InteropServices;
using JetBrains.Annotations;
using PokeSharp.Core;
using PokeSharp.Editor.Core.PokeEdit.Requests;
using PokeSharp.Unreal.Core.Strings;
using PokeSharp.Unreal.Editor.PokeEdit.Requests;
using UnrealSharp.Core;
using UnrealSharp.Core.Marshallers;

namespace PokeSharp.Unreal.Editor.Interop;

[StructLayout(LayoutKind.Sequential)]
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
public unsafe struct PokeEditCallbacks
{
    public required delegate* unmanaged<
        FName,
        FName,
        IntPtr,
        IntPtr,
        int,
        IntPtr,
        UnmanagedArray*,
        NativeBool> SendRequest { get; init; }

    public static PokeEditCallbacks Create()
    {
        return new PokeEditCallbacks { SendRequest = &PokeEditRequestMethods.SendRequest };
    }
}

internal static unsafe class PokeEditRequestMethods
{
    [UnmanagedCallersOnly]
    public static NativeBool SendRequest(
        FName controllerName,
        FName methodName,
        IntPtr request,
        IntPtr requestOffsets,
        int requestOffsetsSize,
        IntPtr response,
        UnmanagedArray* error
    )
    {
        try
        {
            var requestOffsetSpan = new Span<IntPtr>((IntPtr*)requestOffsets, requestOffsetsSize);
            var reader = new UnrealRequestParameterReader(request, requestOffsetSpan);
            var writer = new UnrealResponseWriter(response);

            GameGlobal.PokeEditRequestProcessor.ProcessRequest(
                controllerName.ToPokeSharpName(),
                methodName.ToPokeSharpName(),
                ref reader,
                ref writer
            );
            return NativeBool.True;
        }
        catch (Exception e)
        {
            StringMarshaller.ToNative((IntPtr)error, 0, e.ToString());
            return NativeBool.False;
        }
    }
}
