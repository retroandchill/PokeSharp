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

    public override FieldDefinition GetDefinition(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        JsonSerializerOptions? options = null
    )
    {
        var currentValue = Get(root);
        if (path.Length == 0)
        {
            return new DictionaryFieldDefinition
            {
                FieldId = new PropertySegment(Name),
                Label = DisplayName,
                Tooltip = Tooltip,
                Category = Category,
                IsDefaultValue = DictionariesEqual(currentValue, DefaultValue),
                Pairs =
                [
                    .. currentValue.Select(x =>
                    {
                        var jsonKey = JsonSerializer.SerializeToNode(x.Key, options).RequireNonNull();
                        return new DictionaryFieldPair(
                            this.CreateValueField(KeyType, x.Key, default, new PropertySegment(Name), [], options),
                            this.CreateValueField(
                                ValueType,
                                x.Value,
                                default,
                                new DictionaryKeySegment(jsonKey),
                                [],
                                options
                            )
                        );
                    }),
                ],
                FixedSize = this.TryGetBooleanMetadata(EditablePropertyBuilder.FixedSizeKey),
                MinSize = this.TryGetNumericMetadata<int>(EditablePropertyBuilder.MinSizeKey),
                MaxSize = this.TryGetNumericMetadata<int>(EditablePropertyBuilder.MaxSizeKey),
            };
        }

        if (path[0] is not DictionaryKeySegment keySegment)
        {
            throw new InvalidOperationException(
                $"First segment under list property {Name} must be list index, got {path[0]}"
            );
        }

        var key = keySegment.Key.Deserialize<TKey>(options).RequireNonNull();

        return currentValue.TryGetValue(key, out var value)
            ? this.CreateValueField(ValueType, value, default, keySegment, path, options)
            : throw new InvalidOperationException($"Cannot find key {key} in dictionary.");
    }

    private static bool DictionariesEqual(ImmutableDictionary<TKey, TValue> a, ImmutableDictionary<TKey, TValue>? b)
    {
        if (b is null)
            return false;
        if (a.Count != b.Count)
            return false;

        foreach (var (key, value) in a)
        {
            if (!b.TryGetValue(key, out var otherValue) || !a.ValueComparer.Equals(value, otherValue))
                return false;
        }

        return true;
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
}
