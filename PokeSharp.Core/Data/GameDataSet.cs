using System.Diagnostics.CodeAnalysis;

namespace PokeSharp.Core.Data;

public abstract class GameDataSet<TEntity, TKey>
    where TEntity : IGameDataEntity<TKey, TEntity> 
    where TKey : notnull
{
    protected readonly OrderedDictionary<TKey, TEntity> Data = new();
    
    public IEnumerable<TKey> Keys => Data.Keys;
    
    public IEnumerable<TEntity> Entities => Data.Values;
    
    public int Count => Data.Count;
    
    public bool Exists(TKey key) => Data.ContainsKey(key);

    public TEntity Get(TKey key)
    {
        return Data.TryGetValue(key, out var entity) ? entity : throw new KeyNotFoundException($"Could not find entity with key {key}");
    }
    
    public bool TryGet(TKey key, [NotNullWhen(true)] out TEntity? entity) => Data.TryGetValue(key, out entity);
}

public sealed class RegisteredGameDataSet<TEntity, TKey> : GameDataSet<TEntity, TKey>
    where TEntity : IGameDataEntity<TKey, TEntity>
    where TKey : notnull
{
    public void Register(TEntity entity)
    {
        Data.Add(entity.Id, entity);
    }
}

public sealed class LoadedGameDataSet<TEntity, TKey>(string outputPath) : GameDataSet<TEntity, TKey>
    where TEntity : IGameDataEntity<TKey, TEntity>
    where TKey : notnull
{
    public void Import(IEnumerable<TEntity> entities)
    {
        Data.Clear();
        foreach (var entity in entities)
        {
            Data.Add(entity.Id, entity);
        }
    }
    
    public void Load()
    {
        Data.Clear();
        foreach (var entity in GameContextManager.Current.DataLoader.LoadEntities<TEntity>(outputPath))
        {
            Data.Add(entity.Id, entity);
        }
    }

    public void Save()
    {
        GameContextManager.Current.DataLoader.SaveEntities(Data.Values, outputPath);
    }
}