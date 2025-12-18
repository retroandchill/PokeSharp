using System.Text.Json.Nodes;
using Injectio.Attributes;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Requests;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit;

[RegisterSingleton(ServiceType = typeof(IPokeEditController))]
[PokeEditController]
public sealed partial class EditorController(EditorService editorService)
{
    [PokeEditRequest]
    public IEnumerable<EditorTabOption> GetEditorTabs()
    {
        return editorService.GetEditorTabOptions();
    }

    [PokeEditRequest]
    public IEnumerable<Text> GetEntryLabels(Name editorId)
    {
        return editorService.GetEntryLabels(editorId);
    }

    [PokeEditRequest]
    public JsonNode GetEntryAtIndex(Name editorId, int index)
    {
        return editorService.GetEntryAtIndex(editorId, index);
    }

    [PokeEditRequest]
    public EntityUpdateResponse UpdateEntityAtIndex(Name editorId, int index, ObjectDiffNode change)
    {
        return new EntityUpdateResponse(editorService.UpdateEntity(editorId, index, change));
    }
}
