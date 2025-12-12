using System.Collections.Immutable;
using System.Text.Json.Nodes;
using Injectio.Attributes;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Editors;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit;

[RegisterSingleton]
public class EditorService
{
    private readonly ImmutableDictionary<Name, IEntityEditor> _editors;

    public int EditorCount => _editors.Count;

    public EditorService(IEnumerable<IEntityEditor> editors)
    {
        _editors = editors.ToImmutableDictionary(x => x.Id);
    }

    public IEnumerable<EditorTabOption> GetEditorTabOptions()
    {
        return _editors.Values.Select(x => new EditorTabOption(x.Id, x.Name));
    }

    public IEnumerable<Text> GetEntryLabels(Name editorId)
    {
        return _editors.GetValueOrDefault(editorId)?.SelectionLabels
            ?? throw new InvalidOperationException($"No editor found for {editorId}");
    }

    public JsonNode GetEntryAtIndex(Name editorId, int index)
    {
        return _editors.GetValueOrDefault(editorId)?.GetEntry(index)
            ?? throw new InvalidOperationException($"No editor found for {editorId}");
    }

    public ObjectDiffNode? UpdateEntity(Name editorId, int index, ObjectDiffNode diff)
    {
        return _editors.TryGetValue(editorId, out var editor)
            ? editor.ApplyEdit(index, diff)
            : throw new InvalidOperationException($"No editor found for {editorId}");
    }
}
