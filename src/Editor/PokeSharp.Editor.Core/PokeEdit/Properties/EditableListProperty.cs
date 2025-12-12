using System.Collections.Immutable;
using System.Text.Json;
using PokeSharp.Core.Collections;
using PokeSharp.Core.Utils;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableListProperty : IEditableProperty
{
    IEditableType? ItemType { get; }
    Type ItemClrType { get; }
}

public interface IEditableListProperty<TRoot, TItem>
    : IEditableProperty<TRoot, ImmutableArray<TItem>>,
        IEditableListProperty
    where TRoot : notnull
    where TItem : notnull
{
    new IEditableType<TItem>? ItemType { get; }
    TRoot SetItem(TRoot root, int index, TItem item);
    TRoot Add(TRoot root, TItem item);
    TRoot Insert(TRoot root, int index, TItem item);
    TRoot Swap(TRoot root, int index1, int index2);
    TRoot RemoveAt(TRoot root, int index);
}

public sealed class EditableListProperty<TRoot, TItem>(
    EditableListPropertyBuilder<TRoot, TItem> builder,
    ModelBuildCache cache
) : EditablePropertyBase<TRoot, ImmutableArray<TItem>>(builder), IEditableListProperty<TRoot, TItem>
    where TRoot : notnull
    where TItem : notnull
{
    IEditableType? IEditableListProperty.ItemType => ItemType;
    public IEditableType<TItem>? ItemType { get; } = cache.GetOrBuildType(builder.TargetItemType);
    public Type ItemClrType => typeof(TItem);

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

    public override TRoot ApplyEdit(TRoot root, DiffNode diff, JsonSerializerOptions? options = null)
    {
        return diff switch
        {
            ValueSetNode set => With(root, set.NewValue.Deserialize<ImmutableArray<TItem>>(options).RequireNonNull()),
            ListDiffNode list => ApplyListEdits(root, options, list),
            _ => throw new InvalidOperationException($"Invalid diff node type: {diff.GetType().Name}"),
        };
    }

    private TRoot ApplyListEdits(TRoot root, JsonSerializerOptions? options, ListDiffNode list)
    {
        var newValue = list.Edits.Aggregate(
            Get(root),
            (current, edit) =>
                edit switch
                {
                    ListSetNode set => ApplyListIndexEdit(current, set, options),
                    ListAddNode add => current.Add(add.NewValue.Deserialize<TItem>(options).RequireNonNull()),
                    ListInsertNode ins => current.Insert(
                        ins.Index,
                        ins.NewValue.Deserialize<TItem>(options).RequireNonNull()
                    ),
                    ListRemoveNode rm => current.RemoveAt(rm.Index),
                    ListSwapNode sw => current.Swap(sw.IndexA, sw.IndexB),
                    _ => throw new InvalidOperationException($"Invalid diff node type: {edit.GetType().Name}"),
                }
        );
        return With(root, newValue);
    }

    private ImmutableArray<TItem> ApplyListIndexEdit(
        ImmutableArray<TItem> currentValue,
        ListSetNode set,
        JsonSerializerOptions? options
    )
    {
        if (set.Index < 0 || set.Index >= currentValue.Length)
            throw new InvalidOperationException($"Cannot find index {set.Index} in list.");

        currentValue = set.Change switch
        {
            ValueSetNode valueSet => currentValue.SetItem(
                set.Index,
                valueSet.NewValue.Deserialize<TItem>(options).RequireNonNull()
            ),
            ObjectDiffNode objectDiff when ItemType is not null => currentValue.SetItem(
                set.Index,
                ItemType.ApplyEdit(currentValue[set.Index], objectDiff, options)
            ),
            _ => throw new InvalidOperationException($"Invalid diff node type: {set.Change.GetType().Name}"),
        };
        return currentValue;
    }

    public override DiffNode? Diff(TRoot oldRoot, TRoot newRoot, JsonSerializerOptions? options = null)
    {
        var oldList = Get(oldRoot);
        var newList = Get(newRoot);

        if (oldList == newList)
            return null;

        var oldCount = oldList.Length;
        var newCount = newList.Length;
        var minCount = Math.Min(oldCount, newCount);

        var builder = ImmutableArray.CreateBuilder<ListEditNode>();
        for (var i = 0; i < minCount; i++)
        {
            var oldItem = oldList[i];
            var newItem = newList[i];

            if (EqualityComparer<TItem>.Default.Equals(oldItem, newItem))
                continue;

            if (ItemType is null)
            {
                // No nested editable type; treat as atomic and emit SetValueEdit
                builder.Add(
                    new ListSetNode(
                        i,
                        new ValueSetNode(JsonSerializer.SerializeToNode(newItem, options).RequireNonNull())
                    )
                );
            }
            else
            {
                var diff = ItemType.Diff(oldItem, newItem, options);
                if (diff is not null)
                    builder.Add(new ListSetNode(i, diff));
            }
        }

        // 2. If new list is longer → inserts for [minCount..newCount-1]
        if (newCount > oldCount)
        {
            for (var i = minCount; i < newCount; i++)
            {
                var newItem = newList[i];

                if (i == newCount - 1)
                {
                    builder.Add(new ListAddNode(JsonSerializer.SerializeToNode(newItem, options).RequireNonNull()));
                }
                else
                {
                    builder.Add(
                        new ListInsertNode(i, JsonSerializer.SerializeToNode(newItem, options).RequireNonNull())
                    );
                }
            }
        }
        // 3. If new list is shorter → removes for [newCount..oldCount-1]
        else if (newCount < oldCount)
        {
            // Important: remove from the end backward so indices stay valid.
            for (var i = oldCount - 1; i >= newCount; i--)
            {
                builder.Add(new ListRemoveNode(i));
            }
        }

        return builder.Count != 0
            ? new ListDiffNode(builder.Count == builder.Capacity ? builder.MoveToImmutable() : builder.ToImmutable())
            : null;
    }
}
