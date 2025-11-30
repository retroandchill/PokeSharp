using System.Runtime.InteropServices;

namespace PokeSharp.Unreal.Core.Utils;

[StructLayout(LayoutKind.Sequential)]
public struct SharedPtr
{
    public IntPtr Pointer;
    public IntPtr ReferenceController;
}
