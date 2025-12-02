using System.Text.Json.Nodes;
using Injectio.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Requests;
using PokeSharp.Editor.Core.PokeEdit.Schema;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Editor.Core.PokeEdit;

[RegisterSingleton]
public sealed partial class PokeEditService
{
    private readonly OrderedDictionary<Name, IEntityEditor> _editors = new();

    public PokeEditService(IEnumerable<IEntityEditor> editors)
    {
        foreach (var editor in editors.OrderBy(x => x.Order))
        {
            _editors.Add(editor.Id, editor);
        }
    }

    [CreateSyncVersion]
    [PokeEditRequest]
    public ValueTask<IEnumerable<EditorTabOption>> GetEditorTabsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(_editors.Values.Select(x => new EditorTabOption(x.Id, x.Name)));
    }

    [CreateSyncVersion]
    [PokeEditRequest]
    public ValueTask<TypeDefinition> GetTypeSchemaAsync(Name editorId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(
            _editors.GetValueOrDefault(editorId)?.Type
                ?? throw new InvalidOperationException($"No editor found for {editorId}")
        );
    }

    [CreateSyncVersion]
    [PokeEditRequest]
    public ValueTask<JsonNode?> ProcessFieldEditAsync(FieldEdit edit, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (edit.Path.Segments.Length == 0)
        {
            throw new ArgumentException("Path must have at least one segment");
        }

        if (edit.Path.Segments[0] is not PropertySegment propertySegment)
        {
            throw new ArgumentException("First segment must be a property");
        }

        return _editors.TryGetValue(propertySegment.Name, out var editor)
            ? ValueTask.FromResult(editor.ApplyEdit(edit))
            : throw new InvalidOperationException($"No editor found for {propertySegment.Name}");
    }
}

public static class PokeEditServiceExtensions
{
    [RegisterServices]
    public static void RegisterRequestHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IRequestHandler, PokeEditServiceGetEditorTabsHandler>();
        services.AddSingleton<IRequestHandler, PokeEditServiceGetTypeSchemaHandler>();
    }
}
