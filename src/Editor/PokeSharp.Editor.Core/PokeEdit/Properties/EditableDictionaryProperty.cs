using System.Collections.Immutable;
using System.Text.Json;
using PokeSharp.Core.Utils;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableDictionaryProperty : IEditableProperty
{
    IEditableType? KeyType { get; }
    Type KeyClrType { get; }
    IEditableType? ValueType { get; }
    Type ValueClrType { get; }
}

public interface IEditableDictionaryProperty<TRoot, TKey, TValue>
    : IEditableProperty<TRoot, ImmutableDictionary<TKey, TValue>>,
        IEditableDictionaryProperty
    where TRoot : notnull
    where TKey : notnull
    where TValue : notnull
{
    new IEditableType<TKey>? KeyType { get; }
    new IEditableType<TValue>? ValueType { get; }
    TRoot SetEntry(TRoot root, TKey key, TValue value);
    TRoot RemoveEntry(TRoot root, TKey key);
}

public sealed class EditableDictionaryProperty<TRoot, TKey, TValue>(
    EditableDictionaryPropertyBuilder<TRoot, TKey, TValue> builder,
    ModelBuildCache cache
)
    : EditablePropertyBase<TRoot, ImmutableDictionary<TKey, TValue>>(builder),
        IEditableDictionaryProperty<TRoot, TKey, TValue>
    where TRoot : notnull
    where TKey : notnull
    where TValue : notnull
{
    IEditableType? IEditableDictionaryProperty.KeyType => KeyType;
    public IEditableType<TKey>? KeyType { get; } = cache.GetOrBuildType(builder.TargetKeyType);

    IEditableType? IEditableDictionaryProperty.ValueType => ValueType;
    public IEditableType<TValue>? ValueType { get; } = cache.GetOrBuildType(builder.TargetValueType);

    public Type KeyClrType => typeof(TKey);
    public Type ValueClrType => typeof(TValue);

    public TRoot SetEntry(TRoot root, TKey key, TValue value)
    {
        var dictionary = Get(root);
        return With(root, dictionary.SetItem(key, value));
    }

    public TRoot RemoveEntry(TRoot root, TKey key)
    {
        var dictionary = Get(root);
        return With(root, dictionary.Remove(key));
    }

    public override TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    )
    {
        if (path.Length == 0)
        {
            // Collection-level operations (add/remove/swap...)
            // Similar to your existing ApplyEditToCollection, but scoped to this property
            return edit switch
            {
                DictionarySetEntryEdit set => SetEntry(
                    root,
                    set.Key.Deserialize<TKey>().RequireNonNull(),
                    set.NewValue.Deserialize<TValue>().RequireNonNull()
                ),
                DictionaryRemoveEntryEdit rm => RemoveEntry(root, rm.Key.Deserialize<TKey>().RequireNonNull()),
                _ => throw new InvalidOperationException($"Edit {edit} is not valid for list property {Name}"),
            };
        }

        if (path[0] is not DictionaryKeySegment keySegment)
        {
            throw new InvalidOperationException(
                $"First segment under list property {Name} must be list index, got {path[0]}"
            );
        }

        var key = keySegment.Key.Deserialize<TKey>(options);

        var remaining = path[1..];
        if (remaining.Length == 0)
            return edit is SetValueEdit setEdit
                ? SetEntry(root, key.RequireNonNull(), setEdit.NewValue.Deserialize<TValue>(options).RequireNonNull())
                : throw new InvalidOperationException($"Edit {edit} is not a set operation for list item.");

        if (ValueType is null)
        {
            throw new InvalidOperationException(
                $"Cannot traverse into dictionary value type {typeof(TValue).Name} (no editable properties defined)."
            );
        }

        var dictionary = Get(root);
        return dictionary.TryGetValue(key.RequireNonNull(), out var value)
            ? SetEntry(root, key.RequireNonNull(), ValueType.ApplyEdit(value, remaining, edit, options))
            : throw new InvalidOperationException($"Cannot find key {key} in dictionary.");
    }

    public override void CollectDiffs(
        TRoot oldRoot,
        TRoot newRoot,
        List<FieldEdit> edits,
        FieldPath basePath,
        JsonSerializerOptions? options = null
    )
    {
        var oldDictionary = Get(oldRoot);
        var newDictionary = Get(newRoot);

        if (ReferenceEquals(oldDictionary, newDictionary))
            return;

        var propertyPath = new FieldPath(basePath.Segments.Add(new PropertySegment(Name)));

        foreach (var (key, value) in oldDictionary)
        {
            if (!newDictionary.TryGetValue(key, out var newValue))
            {
                edits.Add(
                    new DictionaryRemoveEntryEdit
                    {
                        Path = propertyPath,
                        Key = JsonSerializer.SerializeToNode(key, options).RequireNonNull(),
                        OriginalValue = JsonSerializer.SerializeToNode(value, options),
                    }
                );
            }
            else if (!EqualityComparer<TValue>.Default.Equals(value, newValue))
            {
                var keyValue = JsonSerializer.SerializeToNode(key, options).RequireNonNull();
                if (ValueType is null)
                {
                    edits.Add(
                        new DictionarySetEntryEdit
                        {
                            Path = propertyPath,
                            Key = keyValue,
                            NewValue = JsonSerializer.SerializeToNode(newValue, options).RequireNonNull(),
                        }
                    );
                }
                else
                {
                    var keyPath = new FieldPath(propertyPath.Segments.Add(new DictionaryKeySegment(keyValue)));
                    ValueType.CollectDiffs(value, newValue, edits, keyPath, options);
                }
            }
        }

        foreach (var (key, value) in newDictionary)
        {
            if (!oldDictionary.ContainsKey(key))
            {
                edits.Add(
                    new DictionarySetEntryEdit
                    {
                        Path = propertyPath,
                        Key = JsonSerializer.SerializeToNode(key, options).RequireNonNull(),
                        NewValue = JsonSerializer.SerializeToNode(value, options).RequireNonNull(),
                    }
                );
            }
        }
    }
}
