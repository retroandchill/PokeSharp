using Injectio.Attributes;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit;

[RegisterSingleton]
public sealed class PokeSharpService
{
    private readonly OrderedDictionary<Name, IEntityEditor> _editors = new();

    public PokeSharpService(IEnumerable<IEntityEditor> editors)
    {
        foreach (var editor in editors.OrderBy(x => x.Order))
        {
            _editors.Add(editor.Id, editor);
        }
    }

    public IEnumerable<OptionItemDefinition> GetEditorTabs()
    {
        return _editors.Values.Select(x => new OptionItemDefinition(x.Id, x.Name));
    }

    public TypeDefinition GetTypeSchema(Name editorId)
    {
        return _editors.GetValueOrDefault(editorId)?.Type
            ?? throw new InvalidOperationException($"No editor found for {editorId}");
    }
}
