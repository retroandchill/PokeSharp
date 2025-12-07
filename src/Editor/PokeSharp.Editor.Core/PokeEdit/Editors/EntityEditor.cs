using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Nodes;
using PokeSharp.Core.Collections;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Properties;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Editors;

public interface IEntityEditor
{
    Name Id { get; }
    Text Name { get; }

    IEnumerable<Text> SelectionLabels { get; }

    IEditableType Type { get; }

    bool DisplayAsSingleton { get; }

    void SyncFromSource();

    FieldDefinition GetField(FieldPathSegment outer, ReadOnlySpan<FieldPathSegment> path);

    JsonNode? ApplyEdit(FieldEdit edit, ReadOnlySpan<FieldPathSegment> path);
}

public abstract class EntityEditor<T>(JsonSerializerOptions options, PokeEditTypeRepository repository) : IEntityEditor
    where T : ILoadedGameDataEntity<T>
{
    private readonly IEditableType<T> _type = repository.GetRequiredType<T>();
    public Name Id => _type.Name;
    public Text Name => _type.DisplayName;

    public abstract IEnumerable<Text> SelectionLabels { get; }

    IEditableType IEntityEditor.Type => _type;

    public bool DisplayAsSingleton
    {
        get => field && _entries.Length == 1;
        protected init;
    }

    private ImmutableArray<T> _entries = [.. T.Entities];

    public void SyncFromSource()
    {
        Interlocked.Exchange(ref _entries, [.. T.Entities]);
    }

    public FieldDefinition GetField(FieldPathSegment outer, ReadOnlySpan<FieldPathSegment> path)
    {
        if (path.Length == 0)
        {
            return new ListFieldDefinition
            {
                FieldId = outer,
                Label = Name,
                ItemFields =
                [
                    .. _entries.Select((x, i) => _type.GetDefinition(x, new ListIndexSegment(i), _type, [], options)),
                ],
            };
        }

        if (path[0] is not ListIndexSegment indexSegment)
        {
            throw new InvalidOperationException($"Must be a list index segment, found {path[0]}");
        }

        var index = indexSegment.Index;
        return index < _entries.Length && index >= 0
            ? _type.GetDefinition(_entries[index], indexSegment, _type, path[1..], options)
            : throw new InvalidOperationException($"Cannot find index {index} in collection.");
    }

    public JsonNode? ApplyEdit(FieldEdit edit, ReadOnlySpan<FieldPathSegment> path)
    {
        if (path.Length == 0)
        {
            return ApplyEditToCollection(edit);
        }

        if (path[0] is not ListIndexSegment indexSegment)
        {
            throw new InvalidOperationException($"Must be a list index segment, found {path[0]}");
        }

        var index = indexSegment.Index;
        var current = _entries[index];
        var remaining = path[1..];

        if (remaining.Length == 0)
        {
            if (edit is not SetValueEdit setValueEdit)
            {
                throw new InvalidOperationException($"Must be performing a set operation, found {edit}");
            }

            var newValue = setValueEdit.NewValue.Deserialize<T>(options);
            if (newValue is null)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize {setValueEdit.NewValue} as {typeof(T).Name}"
                );
            }

            Interlocked.Exchange(ref _entries, _entries.SetItem(indexSegment.Index, newValue));
            return JsonSerializer.SerializeToNode(newValue, options);
        }

        Interlocked.Exchange(ref _entries, _entries.SetItem(index, _type.ApplyEdit(current, remaining, edit, options)));
        return JsonSerializer.SerializeToNode(current, options);
    }

    private JsonNode? ApplyEditToCollection(FieldEdit edit)
    {
        switch (edit)
        {
            case ListAddEdit listAddEdit:
            {
                var newEntry = listAddEdit.NewItem.Deserialize<T>(options);
                if (newEntry is null)
                {
                    throw new InvalidOperationException(
                        $"Failed to deserialize {listAddEdit.NewItem} as {typeof(T).Name}"
                    );
                }

                Interlocked.Exchange(ref _entries, _entries.Add(newEntry));

                return JsonSerializer.SerializeToNode(_entries[^1], options);
            }
            case ListInsertEdit listInsertEdit:
            {
                var newEntry = listInsertEdit.NewItem.Deserialize<T>(options);
                if (newEntry is null)
                {
                    throw new InvalidOperationException(
                        $"Failed to deserialize {listInsertEdit.NewItem} as {typeof(T).Name}"
                    );
                }

                Interlocked.Exchange(ref _entries, _entries.Insert(listInsertEdit.Index, newEntry));

                return JsonSerializer.SerializeToNode(_entries[listInsertEdit.Index], options);
            }
            case ListRemoveAtEdit listRemoveAtEdit:
                Interlocked.Exchange(ref _entries, _entries.RemoveAt(listRemoveAtEdit.Index));
                return null;
            case ListSwapEdit listSwapEdit:
                Interlocked.Exchange(ref _entries, _entries.Swap(listSwapEdit.IndexA, listSwapEdit.IndexB));
                return null;
            default:
                throw new InvalidOperationException($"Cannot perform edit {edit} on a collection.");
        }
    }
}
