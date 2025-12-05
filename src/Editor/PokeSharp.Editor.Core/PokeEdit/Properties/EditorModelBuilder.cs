using System.Collections.Immutable;
using System.Text.Json;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class EditorModelBuilder
{
    private readonly Dictionary<Type, IEditableTypeBuilder> _types = [];

    public EditorModelBuilder For<T>(Action<EditableTypeBuilder<T>>? configure = null)
        where T : notnull
    {
        var builder = GetOrCreateBuilder<T>();
        configure?.Invoke(builder);
        _types.Add(typeof(T), builder);
        return this;
    }

    public EditorModelBuilder Add<T>(IEditableTypeBuilder type)
    {
        _types.Add(typeof(T), type);
        return this;
    }

    internal EditableTypeBuilder<T>? GetBuildIfPossible<T>()
        where T : notnull
    {
        if (
            typeof(T).IsPrimitive
            || typeof(T) == typeof(string)
            || typeof(T).IsEnum
            || typeof(T) == typeof(Name)
            || typeof(T) == typeof(Text)
            || (
                typeof(T).IsGenericType
                && (
                    typeof(T).GetGenericTypeDefinition() == typeof(ImmutableArray<>)
                    || typeof(T).GetGenericTypeDefinition() == typeof(ImmutableDictionary<,>)
                )
            )
        )
        {
            return null;
        }

        return GetOrCreateBuilder<T>();
    }

    internal EditableTypeBuilder<T> GetOrCreateBuilder<T>()
        where T : notnull
    {
        if (_types.TryGetValue(typeof(T), out var typeBuilder))
        {
            return (EditableTypeBuilder<T>)typeBuilder;
        }

        var builder = new EditableTypeBuilder<T>(this);
        foreach (
            var property in typeof(T)
                .GetProperties()
                .Where(x => x is { CanRead: true, CanWrite: true, SetMethod.IsPublic: true })
        )
        {
            builder.Property(property);
        }
        return builder;
    }

    public ImmutableDictionary<Type, IEditableType> Build()
    {
        var cache = new ModelBuildCache();
        foreach (var (_, builder) in _types)
        {
            cache.GetOrBuildType(builder);
        }
        return cache.ExportToImmutable();
    }
}
