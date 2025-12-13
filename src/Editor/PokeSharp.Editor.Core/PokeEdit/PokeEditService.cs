using System.Text.Json.Nodes;
using Injectio.Attributes;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Requests;
using PokeSharp.Editor.Core.PokeEdit.Schema;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Editor.Core.PokeEdit;

[RegisterSingleton(ServiceType = typeof(IApiController))]
[PokeEditController("editor")]
public sealed partial class PokeEditService(EditorService editorService)
{
    [CreateSyncVersion]
    [PokeEditGet("tabs")]
    public ValueTask<IEnumerable<EditorTabOption>> GetEditorTabsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(editorService.GetEditorTabOptions());
    }

    [CreateSyncVersion]
    [PokeEditGet("{editorId}/labels")]
    public ValueTask<IEnumerable<Text>> GetEntryLabelsAsync(
        Name editorId,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(editorService.GetEntryLabels(editorId));
    }

    [CreateSyncVersion]
    [PokeEditGet("{editorId}/{index}")]
    public ValueTask<JsonNode> GetEntryAtIndexAsync(
        Name editorId,
        int index,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(editorService.GetEntryAtIndex(editorId, index));
    }

    [CreateSyncVersion]
    [PokeEditPatch("{editorId}/{index}")]
    public ValueTask<EntityUpdateResponse> UpdateEntityAtIndexAsync(
        Name editorId,
        int index,
        ObjectDiffNode edit,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(new EntityUpdateResponse(editorService.UpdateEntity(editorId, index, edit)));
    }
}
