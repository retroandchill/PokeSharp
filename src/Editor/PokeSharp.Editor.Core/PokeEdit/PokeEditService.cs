using System.Collections.Immutable;
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
    private readonly ImmutableDictionary<Name, IEntityEditor> _editors;

    public int EditorCount => _editors.Count;

    public PokeEditService(IEnumerable<IEntityEditor> editors)
    {
        _editors = editors.ToImmutableDictionary(x => x.Id);
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
        services.AddSingleton<IRequestHandler, PokeEditServiceProcessFieldEditHandler>();
    }
}
