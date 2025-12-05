using Injectio.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;
using PokeSharp.Editor.Core.PokeEdit;
using PokeSharp.Editor.Core.PokeEdit.Properties;

namespace PokeSharp.Editor.PokeEdit;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class DefaultEditorModel : IEditorModelCustomizer
{
    public int Priority => int.MinValue;

    public void OnModelCreating(EditorModelBuilder modelBuilder)
    {
        modelBuilder.For<PokemonType>(type => type.DisplayName(Text.Localized("PokeEdit", "PokemonType", "Types")));
    }
}

public static class DefaultEditorModelExtensions
{
    [RegisterServices]
    public static void AddDefaultEntities(this IServiceCollection services)
    {
        services.AddEntityEditor<PokemonType>();
    }
}
