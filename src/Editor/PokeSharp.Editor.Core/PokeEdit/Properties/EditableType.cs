using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using PokeSharp.Core.Collections.Immutable;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableType : IEditableMember
{
    IEnumerable<IEditableProperty> Properties { get; }

    IEditableProperty GetProperty(Name name);

    IEditableProperty? TryGetProperty(Name name);

    bool TryGetProperty(Name name, [NotNullWhen(true)] out IEditableProperty? property);
}

public interface IEditableType<T> : IEditableType
    where T : notnull
{
    new IEnumerable<IEditableProperty<T>> Properties { get; }

    new IEditableProperty<T> GetProperty(Name name);

    new IEditableProperty<T>? TryGetProperty(Name name);

    bool TryGetProperty(Name name, [NotNullWhen(true)] out IEditableProperty<T>? property);

    T ApplyEdit(T root, ObjectDiffNode diff, JsonSerializerOptions? options = null);

    ObjectDiffNode? Diff(T oldRoot, T newRoot, JsonSerializerOptions? options = null);
}

public sealed class EditableType<T>(EditableTypeBuilder<T> builder, ModelBuildCache cache) : IEditableType<T>
    where T : notnull
{
    public Name Name { get; } = builder.TargetId;
    public Text DisplayName { get; } = builder.TargetDisplayName;
    public Text Tooltip { get; } = builder.TargetTooltip;
    public Text Category { get; } = builder.TargetCategory;

    private readonly ImmutableDictionary<string, string> _metadata = builder.TargetMetadata.ToImmutableDictionary();

    public bool HasMetadata(string key)
    {
        return _metadata.ContainsKey(key);
    }

    public string? GetMetadata(string key)
    {
        return _metadata.GetValueOrDefault(key);
    }

    private readonly ImmutableOrderedDictionary<Name, IEditableProperty<T>> _properties = builder
        .Properties.Values.Select(p => p.Build(cache))
        .ToImmutableOrderedDictionary(x => x.Name);

    public IEnumerable<IEditableProperty<T>> Properties => _properties.Values;

    public IEditableProperty<T> GetProperty(Name name) => _properties[name];

    public IEditableProperty<T>? TryGetProperty(Name name) => _properties.GetValueOrDefault(name);

    public bool TryGetProperty(Name name, [NotNullWhen(true)] out IEditableProperty<T>? property)
    {
        return _properties.TryGetValue(name, out property);
    }

    public T ApplyEdit(T root, ObjectDiffNode diff, JsonSerializerOptions? options = null)
    {
        return diff.Properties.Aggregate(
            root,
            (current, edit) =>
            {
                var property = GetProperty(edit.Key);
                return property.ApplyEdit(current, edit.Value, options);
            }
        );
    }

    public ObjectDiffNode? Diff(T oldRoot, T newRoot, JsonSerializerOptions? options = null)
    {
        var dictionaryBuilder = ImmutableDictionary.CreateBuilder<Name, DiffNode>();
        foreach (var property in Properties)
        {
            var diff = property.Diff(oldRoot, newRoot, options);
            if (diff is not null)
            {
                dictionaryBuilder.Add(property.Name, diff);
            }
        }

        return dictionaryBuilder.Count != 0 ? new ObjectDiffNode(dictionaryBuilder.ToImmutable()) : null;
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
