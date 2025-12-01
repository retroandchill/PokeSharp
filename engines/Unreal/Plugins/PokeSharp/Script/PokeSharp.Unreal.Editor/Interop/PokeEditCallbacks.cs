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
        SharedPtr*,
        SharedPtr*,
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
        FName requestName,
        SharedPtr* request,
        SharedPtr* response,
        UnmanagedArray* error
    )
    {
        try
        {
            using var requestStream = new ArchiveStream(ref *request);
            using var responseStream = new ArchiveStream(ref *response);
            GameGlobal.PokeEditRequestProcessor.ProcessRequest(
                requestName.ToPokeSharpName(),
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
