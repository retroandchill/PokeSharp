using System.Text.Json.Nodes;
using Injectio.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Requests;
using PokeSharp.Editor.Core.PokeEdit.Schema;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Editor.Core.PokeEdit;

[RegisterSingleton]
public sealed partial class PokeEditService(EditorService editorService)
{
    [CreateSyncVersion]
    [PokeEditRequest]
    public ValueTask<IEnumerable<EditorTabOption>> GetEditorTabsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(editorService.GetEditorTabOptions());
    }

    [CreateSyncVersion]
    [PokeEditRequest]
    public ValueTask<IEnumerable<Text>> GetEntryLabelsAsync(
        EditorLabelRequest editorId,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(editorService.GetEntryLabels(editorId.EditorId));
    }

    [CreateSyncVersion]
    [PokeEditRequest]
    public ValueTask<JsonNode> GetEntryAtIndexAsync(
        EntityRequest request,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(editorService.GetEntryAtIndex(request.EditorId, request.Index));
    }

    [CreateSyncVersion]
    [PokeEditRequest]
    public ValueTask<List<FieldEdit>> PerformFieldEditAsync(
        FieldEdit edit,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(editorService.ProcessFieldEdit(edit));
    }
}

public static class PokeEditServiceExtensions
{
    [RegisterServices]
    public static void RegisterRequestHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IRequestHandler, PokeEditServiceGetEditorTabsHandler>();
        services.AddSingleton<IRequestHandler, PokeEditServiceGetEntryLabelsHandler>();
        services.AddSingleton<IRequestHandler, PokeEditServiceGetEntryAtIndexHandler>();
        services.AddSingleton<IRequestHandler, PokeEditServicePerformFieldEditHandler>();
    }
}
