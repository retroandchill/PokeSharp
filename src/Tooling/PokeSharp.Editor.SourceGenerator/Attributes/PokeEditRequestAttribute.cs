#if POKESHARP_EDITOR_GENERATOR
using RhoMicro.CodeAnalysis;
#endif

// ReSharper disable once CheckNamespace
namespace PokeSharp.Editor.Core;

internal enum PokeEditRequestType : byte
{
    Get,
    Post,
    Put,
    Patch,
    Delete,
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
#if POKESHARP_EDITOR_GENERATOR
[IncludeFile]
#endif
internal abstract class PokeEditRequestAttribute(PokeEditRequestType type, string? route = null) : Attribute
{
    public PokeEditRequestType Type { get; } = type;
    public string? Route { get; } = route;
}

internal sealed class PokeEditGetAttribute(string? route) : PokeEditRequestAttribute(PokeEditRequestType.Get, route);

internal sealed class PokeEditPostAttribute(string? route) : PokeEditRequestAttribute(PokeEditRequestType.Post, route);

internal sealed class PokeEditPutAttribute(string? route) : PokeEditRequestAttribute(PokeEditRequestType.Put, route);

internal sealed class PokeEditPatchAttribute(string? route)
    : PokeEditRequestAttribute(PokeEditRequestType.Patch, route);

internal sealed class PokeEditDeleteAttribute(string? route)
    : PokeEditRequestAttribute(PokeEditRequestType.Delete, route);
