using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Properties;

namespace PokeSharp.Editor.Core.PokeEdit.Editors;

public sealed class NamedEntityEditor<T>(JsonSerializerOptions options, PokeEditTypeRepository repository) : EntityEditor<T>(options, repository) where T : ILoadedGameDataEntity<T>, INamedGameDataEntity
{
    public override IEnumerable<Text> SelectionLabels => T.Entities.Select(x => x.Name);
}

public static class NamedEntityEditorExtensions
{
    public static IServiceCollection AddEntityEditor<T>(this IServiceCollection services)
        where T : ILoadedGameDataEntity<T>, INamedGameDataEntity
    {
        return services.AddSingleton<IEntityEditor, NamedEntityEditor<T>>();
    }
}