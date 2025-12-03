using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using PokeSharp.Core.Collections.Immutable;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableType
{
    Name Name { get; }
    IEnumerable<IEditableProperty> Properties { get; }

    IEditableProperty GetProperty(Name name);

    IEditableProperty? TryGetProperty(Name name);

    bool TryGetProperty(Name name, [NotNullWhen(true)] out IEditableProperty? property);
}

public interface IEditableType<T> : IEditableType
{
    new IEnumerable<IEditableProperty<T>> Properties { get; }

    new IEditableProperty<T> GetProperty(Name name);

    new IEditableProperty<T>? TryGetProperty(Name name);

    bool TryGetProperty(Name name, [NotNullWhen(true)] out IEditableProperty<T>? property);

    T ApplyEdit(T root, ReadOnlySpan<FieldPathSegment> path, FieldEdit edit, JsonSerializerOptions? options = null);
}

public sealed class EditableType<T>(Name name, ImmutableOrderedDictionary<Name, IEditableProperty<T>> properties)
    : IEditableType<T>
{
    public Name Name { get; } = name;
    public IEnumerable<IEditableProperty<T>> Properties => properties.Values;

    public IEditableProperty<T> GetProperty(Name name) => properties[name];

    public IEditableProperty<T>? TryGetProperty(Name name) => properties.GetValueOrDefault(name);

    public bool TryGetProperty(Name name, [NotNullWhen(true)] out IEditableProperty<T>? property)
    {
        return properties.TryGetValue(name, out property);
    }

    public T ApplyEdit(
        T root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    )
    {
        if (path.Length == 0)
        {
            throw new InvalidOperationException("Cannot apply edit to root type");
        }

        if (path[0] is not PropertySegment propertySegment)
        {
            throw new InvalidOperationException($"Expected property segment, got {path[0]}");
        }

        return TryGetProperty(propertySegment.Name, out var prop)
            ? prop.ApplyEdit(root, path[1..], edit, options)
            : throw new InvalidOperationException($"No property {propertySegment.Name} on {typeof(T).Name}");
    }

    IEnumerable<IEditableProperty> IEditableType.Properties => Properties;

    IEditableProperty IEditableType.GetProperty(Name name) => GetProperty(name);

    IEditableProperty? IEditableType.TryGetProperty(Name name) => TryGetProperty(name);

    bool IEditableType.TryGetProperty(Name name, [NotNullWhen(true)] out IEditableProperty? property)
    {
        var result = TryGetProperty(name, out var intermediate);
        property = intermediate;
        return result;
    }
}
