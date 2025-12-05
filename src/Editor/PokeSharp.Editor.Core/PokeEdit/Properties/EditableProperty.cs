using System.Collections.Immutable;
using System.Text.Json;
using PokeSharp.Core.Collections;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableProperty
{
    Name Name { get; }
    Text DisplayName { get; }

    FieldDefinition Definition { get; }
}

public interface IEditableProperty<TRoot> : IEditableProperty
    where TRoot : notnull
{
    TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    );
}

public interface IEditableProperty<TRoot, TValue> : IEditableProperty<TRoot>
    where TRoot : notnull
{
    TValue Get(TRoot root);
    TRoot With(TRoot root, TValue value);
}

public interface IEditableObjectProperty<TRoot, TValue> : IEditableProperty<TRoot, TValue>
    where TRoot : notnull
    where TValue : notnull
{
    IEditableType<TValue> Type { get; }
}

public interface IEditableListProperty<TRoot, TItem> : IEditableProperty<TRoot, ImmutableArray<TItem>>
    where TRoot : notnull
    where TItem : notnull
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
    where TRoot : notnull
    where TKey : notnull
    where TValue : notnull
{
    IEditableType<TValue>? ValueType { get; }
    TRoot SetEntry(TRoot root, TKey key, TValue value);
    TRoot RemoveEntry(TRoot root, TKey key);
}

public abstract class EditablePropertyBase<TRoot, TValue>(EditablePropertyBuilder<TRoot, TValue> builder)
    : IEditableProperty<TRoot, TValue>
    where TRoot : notnull
{
    public Name Name { get; } = builder.TargetName;
    public Text DisplayName { get; } = builder.TargetDisplayName;
    private readonly Func<TRoot, TValue> _get = builder.TargetGet;
    private readonly Func<TRoot, TValue, TRoot> _with = builder.TargetWith;

    public FieldDefinition Definition { get; } = builder.BuildFieldDefinition();

    public TValue Get(TRoot root) => _get(root);

    public TRoot With(TRoot root, TValue value) => _with(root, value);

    public abstract TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    );
}

public sealed class EditableScalarProperty<TRoot, TValue>(EditableScalarPropertyBuilder<TRoot, TValue> builder)
    : EditablePropertyBase<TRoot, TValue>(builder)
    where TRoot : notnull
{
    public override TRoot ApplyEdit(
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

public sealed class EditableObjectProperty<TRoot, TValue>(
    EditableObjectPropertyBuilder<TRoot, TValue> builder,
    ModelBuildCache cache
) : EditablePropertyBase<TRoot, TValue>(builder), IEditableObjectProperty<TRoot, TValue>
    where TRoot : notnull
    where TValue : notnull
{
    public IEditableType<TValue> Type { get; } = cache.GetOrBuildType(builder.TargetType);

    public override TRoot ApplyEdit(
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

public sealed class EditableListProperty<TRoot, TItem>(
    EditableListPropertyBuilder<TRoot, TItem> builder,
    ModelBuildCache cache
) : EditablePropertyBase<TRoot, ImmutableArray<TItem>>(builder), IEditableListProperty<TRoot, TItem>
    where TRoot : notnull
    where TItem : notnull
{
    public IEditableType<TItem>? ItemType { get; } = cache.GetOrBuildType(builder.TargetItemType);

    public TRoot SetItem(TRoot root, int index, TItem item)
    {
        var list = Get(root);
        return index >= 0 && index < list.Length
            ? With(root, list.SetItem(index, item))
            : throw new InvalidOperationException($"Cannot find index {index} in list.");
    }

    public TRoot Add(TRoot root, TItem item)
    {
        var list = Get(root);
        return With(root, list.Add(item));
    }

    public TRoot Insert(TRoot root, int index, TItem item)
    {
        var list = Get(root);
        return With(root, list.Insert(index, item));
    }

    public TRoot Swap(TRoot root, int index1, int index2)
    {
        var list = Get(root);
        return With(root, list.Swap(index1, index2));
    }

    public TRoot RemoveAt(TRoot root, int index)
    {
        var list = Get(root);
        return With(root, list.RemoveAt(index));
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
    public IEditableType<TValue>? ValueType { get; } = cache.GetOrBuildType(builder.TargetValueType);

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
