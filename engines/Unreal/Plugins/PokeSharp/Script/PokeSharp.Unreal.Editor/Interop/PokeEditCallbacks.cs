using System.Runtime.InteropServices;
using JetBrains.Annotations;
using PokeSharp.Core;
using PokeSharp.Editor.Core.PokeEdit.Requests;
using PokeSharp.Unreal.Core.Serialization;
using PokeSharp.Unreal.Core.Strings;
using PokeSharp.Unreal.Core.Utils;
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
        IntPtr response,
        UnmanagedArray* error
    )
    {
        try
        {
            GameGlobal.PokeEditRequestProcessor.ProcessRequest(
                controllerName.ToPokeSharpName(),
                methodName.ToPokeSharpName(),
                requestStream,
                responseStream
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
