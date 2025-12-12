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

    ObjectDiffNode? ApplyEdit(int index, ObjectDiffNode diff);
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

    public ObjectDiffNode? ApplyEdit(int index, ObjectDiffNode diff)
    {
        if (index < 0 || index >= _entries.Length)
        {
            throw new InvalidOperationException($"Cannot find index {index} in collection.");
        }

        var current = _entries[index];
        ImmutableInterlocked.InterlockedExchange(
            ref _entries,
            _entries.SetItem(index, _type.ApplyEdit(current, diff, options))
        );

        return _type.Diff(current, _entries[index], options);
    }
}
