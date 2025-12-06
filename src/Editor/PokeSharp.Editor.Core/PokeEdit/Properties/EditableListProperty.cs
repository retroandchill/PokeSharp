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

    public override FieldDefinition GetDefinition(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        JsonSerializerOptions? options = null
    )
    {
        var currentValue = Get(root);
        if (path.Length == 0)
        {
            return new ListFieldDefinition
            {
                FieldId = new PropertySegment(Name),
                Label = DisplayName,
                Tooltip = Tooltip,
                Category = Category,
                IsDefaultValue = currentValue.SequenceEqual(DefaultValue),
                ItemFields =
                [
                    .. currentValue.Select(
                        (item, i) =>
                            this.CreateValueField(ItemType, item, default, new ListIndexSegment(i), [], options)
                    ),
                ],
                FixedSize = this.TryGetBooleanMetadata(EditablePropertyBuilder.FixedSizeKey),
                MinSize = this.TryGetNumericMetadata<int>(EditablePropertyBuilder.MinSizeKey),
                MaxSize = this.TryGetNumericMetadata<int>(EditablePropertyBuilder.MaxSizeKey),
            };
        }

        if (path[0] is not ListIndexSegment indexSegment)
        {
            throw new InvalidOperationException(
                $"First segment under list property {Name} must be list index, got {path[0]}"
            );
        }

        var index = indexSegment.Index;
        return currentValue.Length > index || index < 0
            ? this.CreateValueField(ItemType, currentValue[index], default, indexSegment, path, options)
            : throw new InvalidOperationException($"Cannot find index {index} in list.");
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
                ListAddEdit add => Add(root, add.NewItem.Deserialize<TItem>().RequireNonNull()),
                ListInsertEdit ins => Insert(root, ins.Index, ins.NewItem.Deserialize<TItem>().RequireNonNull()),
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
                ? SetItem(root, index, setEdit.NewValue.Deserialize<TItem>(options).RequireNonNull())
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
