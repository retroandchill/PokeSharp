using PokeSharp.Editor.Core;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;

namespace PokeSharp.Editor.SourceGenerator.Model;

[AttributeInfoType<PokeEditRequestAttribute>]
internal record PokeEditRequestInfo(PokeEditRequestType Type, string? Route);

[AttributeInfoType<PokeEditGetAttribute>]
internal sealed record PokeEditGetInfo(string? Route) : PokeEditRequestInfo(PokeEditRequestType.Get, Route);

[AttributeInfoType<PokeEditPostAttribute>]
internal sealed record PokeEditPostInfo(string? Route) : PokeEditRequestInfo(PokeEditRequestType.Post, Route);

[AttributeInfoType<PokeEditPutAttribute>]
internal sealed record PokeEditPutInfo(string? Route) : PokeEditRequestInfo(PokeEditRequestType.Put, Route);

[AttributeInfoType<PokeEditPatchAttribute>]
internal sealed record PokeEditPatchInfo(string? Route) : PokeEditRequestInfo(PokeEditRequestType.Patch, Route);

[AttributeInfoType<PokeEditDeleteAttribute>]
internal sealed record PokeEditDeleteInfo(string? Route) : PokeEditRequestInfo(PokeEditRequestType.Delete, Route);
