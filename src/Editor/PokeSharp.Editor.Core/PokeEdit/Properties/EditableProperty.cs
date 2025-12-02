using System.Collections.Immutable;
using System.Text.Json;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableProperty
{
    Name Name { get; }
}

public interface IEditableProperty<TRoot> : IEditableProperty
{
    TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    );
}

public interface IEditableProperty<TRoot, TValue> : IEditableProperty<TRoot>
{
    TValue Get(TRoot root);
    TRoot With(TRoot root, TValue value);
}

public interface IEditableObjectProperty<TRoot, TValue> : IEditableProperty<TRoot, TValue>
{
    IEditableType<TValue> Type { get; }
}

public interface IEditableListProperty<TRoot, TItem> : IEditableProperty<TRoot, ImmutableArray<TItem>>
{
    IEditableType<TItem>? ItemType { get; }
    TRoot SetItem(TRoot root, int index, TItem item);
    TRoot Add(TRoot root, TItem item);
    TRoot Insert(TRoot root, int index, TItem item);
    TRoot Swap(TRoot root, int index1, int index2);
    TRoot RemoveAt(TRoot root, int index);
}

public interface IEditableDictionaryProperty<TRoot, TKey, TValue>
    : IEditableProperty<TRoot, ImmutableDictionary<TKey, TValue>>
    where TKey : notnull
{
    IEditableType<TValue>? ValueType { get; }
    TRoot SetEntry(TRoot root, TKey key, TValue value);
    TRoot RemoveEntry(TRoot root, TKey key);
}

public abstract class EditableScalarProperty<TRoot, TValue> : IEditableProperty<TRoot, TValue>
{
    public abstract Name Name { get; }
    public abstract TValue Get(TRoot root);
    public abstract TRoot With(TRoot root, TValue value);

    public TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    )
    {
        if (path.Length != 0)
        {
            throw new InvalidOperationException(
                $"Cannot traverse into scalar property {Name}, path still has {path.Length} segment(s)."
            );
        }

        return edit is SetValueEdit set
            ? With(root, set.NewValue.Deserialize<TValue>(options)!)
            : throw new InvalidOperationException($"Edit {edit} is not valid for scalar property {Name}");
    }
}

public abstract class EditableObjectProperty<TRoot, TValue> : IEditableObjectProperty<TRoot, TValue>
{
    public abstract Name Name { get; }
    public abstract IEditableType<TValue> Type { get; }
    public abstract TValue Get(TRoot root);
    public abstract TRoot With(TRoot root, TValue value);

    public TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    )
    {
        if (path.Length != 0)
        {
            return With(root, Type.ApplyEdit(Get(root), path, edit, options));
        }

        return edit is SetValueEdit set
            ? With(root, set.NewValue.Deserialize<TValue>(options)!)
            : throw new InvalidOperationException($"Edit {edit} is not valid for scalar property {Name}");
    }
}

public abstract class EditableListProperty<TRoot, TItem> : IEditableListProperty<TRoot, TItem>
{
    public abstract Name Name { get; }
    public abstract IEditableType<TItem>? ItemType { get; }
    public abstract ImmutableArray<TItem> Get(TRoot root);
    public abstract TRoot With(TRoot root, ImmutableArray<TItem> value);
    public abstract TRoot SetItem(TRoot root, int index, TItem item);
    public abstract TRoot Add(TRoot root, TItem item);
    public abstract TRoot Insert(TRoot root, int index, TItem item);
    public abstract TRoot Swap(TRoot root, int index1, int index2);
    public abstract TRoot RemoveAt(TRoot root, int index);

    public TRoot ApplyEdit(
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
                ListAddEdit add => Add(root, add.NewItem.Deserialize<TItem>()!),
                ListInsertEdit ins => Insert(root, ins.Index, ins.NewItem.Deserialize<TItem>()!),
                ListRemoveAtEdit rm => RemoveAt(root, rm.Index),
                ListSwapEdit sw => Swap(root, sw.IndexA, sw.IndexB),
                _ => throw new InvalidOperationException($"Edit {edit} is not valid for list property {Name}"),
            };
        }

        if (path[0] is not ListIndexSegment indexSegment)
        {
            throw new InvalidOperationException(
                $"First segment under list property {Name} must be list index, got {path[0]}"
            );
        }

        var index = indexSegment.Index;

        // If you want to support nesting *into* list items (i.e., item has its own editable properties),
        // you’d inject a property map for TItem and recurse into it here.
        // For now, treat item as scalar: remaining path must be empty.
        var remaining = path[1..];
        if (remaining.Length == 0)
            return edit is SetValueEdit setEdit
                ? SetItem(root, index, setEdit.NewValue.Deserialize<TItem>(options)!)
                : throw new InvalidOperationException($"Edit {edit} is not a set operation for list item.");

        if (ItemType is null)
        {
            throw new InvalidOperationException(
                $"Cannot traverse into list item type {typeof(TItem).Name} (no editable properties defined)."
            );
        }

        var list = Get(root);
        return list.Length > index || index < 0
            ? SetItem(root, index, ItemType.ApplyEdit(list[index], remaining, edit, options))
            : throw new InvalidOperationException($"Cannot find index {index} in list.");
    }
}

public abstract class EditableDictionaryProperty<TRoot, TKey, TValue> : IEditableDictionaryProperty<TRoot, TKey, TValue>
    where TKey : notnull
{
    public abstract Name Name { get; }
    public abstract IEditableType<TValue>? ValueType { get; }
    public abstract ImmutableDictionary<TKey, TValue> Get(TRoot root);
    public abstract TRoot With(TRoot root, ImmutableDictionary<TKey, TValue> value);
    public abstract TRoot SetEntry(TRoot root, TKey key, TValue value);
    public abstract TRoot RemoveEntry(TRoot root, TKey key);

    public TRoot ApplyEdit(
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
                    set.Key.Deserialize<TKey>()!,
                    set.NewValue.Deserialize<TValue>()!
                ),
                DictionaryRemoveEntryEdit rm => RemoveEntry(root, rm.Key.Deserialize<TKey>()!),
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
                ? SetEntry(root, key!, setEdit.NewValue.Deserialize<TValue>(options)!)
                : throw new InvalidOperationException($"Edit {edit} is not a set operation for list item.");

        if (ValueType is null)
        {
            throw new InvalidOperationException(
                $"Cannot traverse into dictionary value type {typeof(TValue).Name} (no editable properties defined)."
            );
        }

        var dictionary = Get(root);
        return dictionary.TryGetValue(key!, out var value)
            ? SetEntry(root, key!, ValueType.ApplyEdit(value, remaining, edit, options))
            : throw new InvalidOperationException($"Cannot find key {key} in dictionary.");
    }
}
