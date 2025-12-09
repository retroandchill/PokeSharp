using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Nodes;
using PokeSharp.Core.Collections;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;
using PokeSharp.Core.Utils;
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

    JsonNode GetEntry(int index);

    List<FieldEdit> ApplyEdit(FieldEdit edit, ReadOnlySpan<FieldPathSegment> path);
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

    public JsonNode GetEntry(int index)
    {
        return index < _entries.Length && index >= 0
            ? JsonSerializer.SerializeToNode(_entries[index], options).RequireNonNull()
            : throw new InvalidOperationException($"Cannot find index {index} in collection.");
    }

    public List<FieldEdit> ApplyEdit(FieldEdit edit, ReadOnlySpan<FieldPathSegment> path)
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
        }

        Interlocked.Exchange(ref _entries, _entries.SetItem(index, _type.ApplyEdit(current, remaining, edit, options)));

        var result = new List<FieldEdit>();
        _type.CollectDiffs(current, _entries[index], result, new FieldPath([.. path]), options);
        return result;
    }

    private List<FieldEdit> ApplyEditToCollection(FieldEdit edit)
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
                break;
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
                break;
            }
            case ListRemoveAtEdit listRemoveAtEdit:
                Interlocked.Exchange(ref _entries, _entries.RemoveAt(listRemoveAtEdit.Index));
                break;
            case ListSwapEdit listSwapEdit:
                Interlocked.Exchange(ref _entries, _entries.Swap(listSwapEdit.IndexA, listSwapEdit.IndexB));
                break;
            default:
                throw new InvalidOperationException($"Cannot perform edit {edit} on a collection.");
        }

        return [edit];
    }
}
