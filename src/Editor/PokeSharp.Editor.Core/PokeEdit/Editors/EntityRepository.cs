using System.Text.Json;
using PokeSharp.Core.Collections.Immutable;
using PokeSharp.Core.Data;
using PokeSharp.Editor.Core.PokeEdit.Properties;
using PokeSharp.Editor.Core.PokeEdit.Schema;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Editor.Core.PokeEdit.Editors;

public interface IEntityRepository
{
    IEditableType Type { get; }
    
    bool HasPendingChanges { get; }
    
    void SyncFromSource();

    ObjectDiffNode? ApplyEditAt(int index, ObjectDiffNode diff);
    
    void Swap(int index1, int index2);
    
    void RemoveAt(int index);

    void SaveChanges();
    
    ValueTask SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IEntityRepository<TEntity> : IEntityRepository where TEntity : ILoadedGameDataEntity<TEntity>
{
    new IEditableType<TEntity> Type { get; }
    
    TEntity GetEntryAt(int index);
}

public interface IEntityRepository<TKey, TEntity> : IEntityRepository<TEntity> where TKey : notnull where TEntity : ILoadedGameDataEntity<TKey, TEntity>
{
    new IEditableType<TEntity> Type { get; }
    
    public ImmutableOrderedDictionary<TKey, TEntity> Entries { get;}
    
    TEntity GetEntry(TKey key);

    ObjectDiffNode? ApplyEdit(TKey key, ObjectDiffNode diff);
    
    void Remove(TKey key);
}

public abstract partial class EntityRepository<TKey, TEntity>(JsonSerializerOptions options, LoadedGameDataSet<TEntity, TKey> dataSet, PokeEditTypeRepository repository) : IEntityRepository<TKey, TEntity>
    where TKey : notnull
    where TEntity : ILoadedGameDataEntity<TKey, TEntity> 
{
    [Flags]
    private enum SaveState : byte
    {
        None = 0,
        Pending = 1,
        Saving = 2
    }
    
    public IEditableType<TEntity> Type { get; } = repository.GetRequiredType<TEntity>();

    IEditableType IEntityRepository.Type => Type;

    public ImmutableOrderedDictionary<TKey, TEntity> Entries
    {
        get;
        private set
        {
            if (ReferenceEquals(field, value)) return;
            
            Interlocked.Exchange(ref field, value);
            dataSet.Import(field, false);
            SaveStateStatus |= SaveState.Pending;
        }
    } = dataSet.Data;

    private SaveState SaveStateStatus
    {
        get;
        set => Interlocked.Exchange(ref field, value);
    } = SaveState.None;
    
    public bool HasPendingChanges => SaveStateStatus.HasFlag(SaveState.Pending);

    public void SyncFromSource()
    {
        Entries = dataSet.Data;
    }

    public TEntity GetEntry(TKey key)
    {
        return Entries.TryGetValue(key, out var entry) ? entry : throw new InvalidOperationException($"Cannot find entry with key {key}.");
    }

    public TEntity GetEntryAt(int index)
    {
        return index < Entries.Count && index >= 0
            ? Entries.GetAt(index).Value
            : throw new InvalidOperationException($"Cannot find index {index} in collection.");
    }
    
    public ObjectDiffNode? ApplyEdit(TKey key, ObjectDiffNode diff)
    {
        if (!Entries.TryGetValue(key, out var current))
        {
            throw new InvalidOperationException($"Cannot find key {key} in collection.");
        }

        var newValue = Type.ApplyEdit(current, diff, options);
        var diffResult = Type.Diff(current, newValue, options);
        if (diffResult is null) return null;
        
        
        Entries = Entries.SetItem(key, newValue);
        return diffResult;
    }

    public ObjectDiffNode? ApplyEditAt(int index, ObjectDiffNode diff)
    {
        if (index < 0 || index >= Entries.Count)
        {
            throw new InvalidOperationException($"Cannot find index {index} in collection.");
        }

        var current = Entries.GetAt(index).Value;
        var newValue = Type.ApplyEdit(current, diff, options);
        var diffResult = Type.Diff(current, newValue, options);
        if (diffResult is null) return null;
        
        Entries = Entries.SetAt(index, newValue);
        return diffResult;
    }

    public void Swap(int index1, int index2)
    {
        if (index1 < 0 || index1 >= Entries.Count)
        {
            throw new InvalidOperationException($"Cannot find index {index1} in collection.");
        }
        
        if (index2 < 0 || index2 >= Entries.Count)
        {
            throw new InvalidOperationException($"Cannot find index {index2} in collection.");
        }

        if (index1 == index2) return;
        
        Entries = Entries.Swap(index1, index2);
    }

    public void Remove(TKey key)
    {
        var newEntries = Entries.Remove(key);
        if (ReferenceEquals(Entries, newEntries))
        {
            throw new InvalidOperationException($"Cannot find key {key} in collection.");
        }
        
        Entries = newEntries;
        dataSet.Import(Entries, false);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Entries.Count)
        {
            throw new InvalidOperationException($"Cannot find index {index} in collection.");
        }
        
        Entries = Entries.RemoveAt(index);
        dataSet.Import(Entries, false);
    }

    [CreateSyncVersion]
    public async ValueTask SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (SaveStateStatus.HasFlag(SaveState.Saving)) throw new InvalidOperationException("Cannot save changes while already saving.");

        if (!SaveStateStatus.HasFlag(SaveState.Pending)) return;

        SaveStateStatus |= SaveState.Saving;
        await dataSet.SaveAsync(cancellationToken);
        SaveStateStatus &= ~SaveState.Pending;
    }
}
