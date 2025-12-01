using Injectio.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Requests;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit;

[RegisterSingleton]
public sealed class PokeEditService
{
    private readonly OrderedDictionary<Name, IEntityEditor> _editors = new();

    public PokeEditService(IEnumerable<IEntityEditor> editors)
    {
        foreach (var editor in editors.OrderBy(x => x.Order))
        {
            _editors.Add(editor.Id, editor);
        }
    }

    [PokeEditRequest]
    public IEnumerable<OptionItemDefinition> GetEditorTabs()
    {
        return _editors.Values.Select(x => new OptionItemDefinition(x.Id, x.Name));
    }

    [PokeEditRequest]
    public TypeDefinition GetTypeSchema(Name editorId)
    {
        return _editors.GetValueOrDefault(editorId)?.Type
            ?? throw new InvalidOperationException($"No editor found for {editorId}");
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
