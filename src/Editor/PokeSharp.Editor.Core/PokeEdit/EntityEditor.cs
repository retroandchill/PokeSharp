using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using PokeSharp.Core.Collections;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Properties;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit;

public interface IEntityEditor
{
    Name Id { get; }
    Text Name { get; }

    TypeDefinition Type { get; }
    IEditableType Properties { get; }

    JsonNode GetDefaultValue();

    void SyncFromSource();

    JsonNode? ApplyEdit(FieldEdit edit);
}

public abstract class EntityEditor<T>(JsonSerializerOptions options) : IEntityEditor
    where T : ILoadedGameDataEntity<T>
{
    public abstract Name Id { get; }
    public abstract Text Name { get; }
    public abstract TypeDefinition Type { get; }
    IEditableType IEntityEditor.Properties => Properties;
    protected abstract IEditableType<T> Properties { get; }

    protected abstract T DefaultEntry { get; }

    private ImmutableArray<T> _entries = [];

    public JsonNode GetDefaultValue()
    {
        return JsonSerializer.SerializeToNode(DefaultEntry, options) ?? throw new InvalidOperationException();
    }

    public void SyncFromSource()
    {
        Interlocked.Exchange(ref _entries, [.. T.Entities]);
    }

    public JsonNode? ApplyEdit(FieldEdit edit)
    {
        if (edit.Path.Segments.Length == 0)
        {
            return ApplyEditToCollection(edit);
        }

        if (edit.Path.Segments[0] is not ListIndexSegment indexSegment)
        {
            throw new InvalidOperationException($"Must be a list index segment, found {edit.Path.Segments[0]}");
        }

        var index = indexSegment.Index;
        var current = _entries[index];
        var remaining = edit.Path.Segments.AsSpan()[1..];

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

        Interlocked.Exchange(
            ref _entries,
            _entries.SetItem(index, Properties.ApplyEdit(current, remaining, edit, options))
        );
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
