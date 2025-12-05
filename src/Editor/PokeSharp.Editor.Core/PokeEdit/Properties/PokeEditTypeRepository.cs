using System.Collections.Immutable;
using Injectio.Attributes;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

[RegisterSingleton]
public class PokeEditTypeRepository
{
    private readonly ImmutableDictionary<Type, IEditableType> _types;

    public PokeEditTypeRepository(IEnumerable<IEditorModelCustomizer> customizers)
    {
        var builder = new EditorModelBuilder();
        foreach (var customizer in customizers.OrderBy(x => x.Priority))
        {
            customizer.OnModelCreating(builder);
        }
        _types = builder.Build();
    }

    public IEditableType? GetType(Type type)
    {
        return _types.GetValueOrDefault(type);
    }

    public IEditableType<T>? GetType<T>()
        where T : notnull
    {
        return (IEditableType<T>?)GetType(typeof(T));
    }

    public IEditableType<T> GetRequiredType<T>()
        where T : notnull
    {
        return GetType<T>() ?? throw new InvalidOperationException($"Cannot find type {typeof(T)}");
    }
}
