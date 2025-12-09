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

    public List<FieldEdit> ProcessFieldEdit(FieldEdit edit)
    {
        if (edit.Path.Segments.Length == 0)
        {
            throw new ArgumentException("Path must have at least one segment");
        }

        if (edit.Path.Segments[0] is not PropertySegment propertySegment)
        {
            throw new ArgumentException("First segment must be a property");
        }

        return _editors.TryGetValue(propertySegment.Name, out var editor)
            ? editor.ApplyEdit(edit, edit.Path.Segments.AsSpan()[1..])
            : throw new InvalidOperationException($"No editor found for {propertySegment.Name}");
    }
}
