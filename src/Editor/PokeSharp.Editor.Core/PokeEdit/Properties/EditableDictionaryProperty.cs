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

    public override TRoot ApplyEdit(TRoot root, DiffNode diff, JsonSerializerOptions? options = null)
    {
        return diff switch
        {
            ValueSetNode set => With(
                root,
                set.NewValue.Deserialize<ImmutableDictionary<TKey, TValue>>(options).RequireNonNull()
            ),
            DictionaryDiffNode dict => ApplyDictionaryEdits(root, dict, options),
            _ => throw new InvalidOperationException($"Invalid diff node type: {diff.GetType().Name}"),
        };
    }

    private TRoot ApplyDictionaryEdits(TRoot root, DictionaryDiffNode dict, JsonSerializerOptions? options)
    {
        var newDictionary = dict.Edits.Aggregate(
            Get(root),
            (current, edit) =>
                edit switch
                {
                    DictionarySetNode set => ApplyDictionaryIndexEdit(current, set, options),
                    DictionaryAddNode add => current.SetItem(
                        add.Key.Deserialize<TKey>(options).RequireNonNull(),
                        add.Value.Deserialize<TValue>(options).RequireNonNull()
                    ),
                    DictionaryRemoveNode rm => current.Remove(rm.Key.Deserialize<TKey>(options).RequireNonNull()),
                    DictionaryChangeKeyNode changeKey => ApplyChangeKeyEdit(current, changeKey, options),
                    _ => throw new InvalidOperationException($"Invalid diff node type: {edit.GetType().Name}"),
                }
        );
        return With(root, newDictionary);
    }

    private ImmutableDictionary<TKey, TValue> ApplyDictionaryIndexEdit(
        ImmutableDictionary<TKey, TValue> currentValue,
        DictionarySetNode set,
        JsonSerializerOptions? options
    )
    {
        var key = set.Key.Deserialize<TKey>(options).RequireNonNull();
        if (!currentValue.TryGetValue(key, out var value))
            throw new InvalidOperationException($"Cannot find key {key} in dictionary.");

        currentValue = set.Change switch
        {
            ValueSetNode valueSet => currentValue.SetItem(
                key,
                valueSet.NewValue.Deserialize<TValue>(options).RequireNonNull()
            ),
            ObjectDiffNode objectDiff when ValueType is not null => currentValue.SetItem(
                key,
                ValueType.ApplyEdit(value, objectDiff, options)
            ),
            _ => throw new InvalidOperationException($"Invalid diff node type: {set.Change.GetType().Name}"),
        };
        return currentValue;
    }

    private ImmutableDictionary<TKey, TValue> ApplyChangeKeyEdit(
        ImmutableDictionary<TKey, TValue> currentValue,
        DictionaryChangeKeyNode changeKey,
        JsonSerializerOptions? options
    )
    {
        var oldKey = changeKey.OldKey.Deserialize<TKey>(options).RequireNonNull();
        return currentValue.TryGetValue(oldKey, out var value)
            ? currentValue.SetItem(changeKey.NewKey.Deserialize<TKey>(options).RequireNonNull(), value)
            : throw new InvalidOperationException($"Cannot find key {oldKey} in dictionary.");
    }

    public override DiffNode? Diff(TRoot oldRoot, TRoot newRoot, JsonSerializerOptions? options = null)
    {
        var oldDictionary = Get(oldRoot);
        var newDictionary = Get(newRoot);

        if (ReferenceEquals(oldDictionary, newDictionary))
            return null;

        var builder = ImmutableArray.CreateBuilder<DictionaryEditNode>();
        foreach (var (key, value) in oldDictionary)
        {
            if (!newDictionary.TryGetValue(key, out var newValue))
            {
                builder.Add(new DictionaryRemoveNode(JsonSerializer.SerializeToNode(key, options).RequireNonNull()));
            }
            else if (!EqualityComparer<TValue>.Default.Equals(value, newValue))
            {
                var keyValue = JsonSerializer.SerializeToNode(key, options).RequireNonNull();
                if (ValueType is null)
                {
                    builder.Add(
                        new DictionarySetNode(
                            keyValue,
                            new ValueSetNode(JsonSerializer.SerializeToNode(newValue, options).RequireNonNull())
                        )
                    );
                }
                else
                {
                    var diff = ValueType.Diff(value, newValue, options);
                    if (diff is not null)
                    {
                        builder.Add(new DictionarySetNode(keyValue, diff));
                    }
                }
            }
        }

        foreach (var (key, value) in newDictionary)
        {
            if (!oldDictionary.ContainsKey(key))
            {
                builder.Add(
                    new DictionaryAddNode(
                        JsonSerializer.SerializeToNode(key, options).RequireNonNull(),
                        JsonSerializer.SerializeToNode(value, options).RequireNonNull()
                    )
                );
            }
        }

        return builder.Count != 0
            ? new DictionaryDiffNode(
                builder.Count == builder.Capacity ? builder.MoveToImmutable() : builder.ToImmutable()
            )
            : null;
    }
}
