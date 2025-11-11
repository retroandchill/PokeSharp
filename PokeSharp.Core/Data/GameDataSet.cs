using System.Diagnostics.CodeAnalysis;

namespace PokeSharp.Core.Data;

public abstract class GameDataSet<TEntity, TKey>
    where TEntity : IGameDataEntity<TKey, TEntity>
    where TKey : notnull
{
    private OrderedDictionary<TKey, TEntity> _data = new();

    protected OrderedDictionary<TKey, TEntity> Data => _data;

    public IEnumerable<TKey> Keys => _data.Keys;

    public IEnumerable<TEntity> Entities => _data.Values;

    public int Count => _data.Count;

    public bool Exists(TKey key) => _data.ContainsKey(key);

    public TEntity Get(TKey key)
    {
        return _data.TryGetValue(key, out var entity)
            ? entity
            : throw new KeyNotFoundException($"Could not find entity with key {key}");
    }

    public bool TryGet(TKey key, [NotNullWhen(true)] out TEntity? entity) =>
        _data.TryGetValue(key, out entity);

    protected void ReplaceData(IEnumerable<TEntity> entities)
    {
        var newData = new OrderedDictionary<TKey, TEntity>();
        foreach (var entity in entities)
        {
            newData.Add(entity.Id, entity);
        }

        Interlocked.Exchange(ref _data, newData);
    }

    protected async ValueTask ReplaceDataAsync(IAsyncEnumerable<TEntity> entities)
    {
        var newData = new OrderedDictionary<TKey, TEntity>();
        await foreach (var entity in entities)
        {
            newData.Add(entity.Id, entity);
        }

        Interlocked.Exchange(ref _data, newData);
    }
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
    public void Import(IEnumerable<TEntity> entities) => ReplaceData(entities);

    public ValueTask Load(CancellationToken cancellationToken = default)
    {
        return ReplaceDataAsync(
            GameContextManager.Current.DataLoader.LoadEntities<TEntity>(
                outputPath,
                cancellationToken
            )
        );
    }

    public ValueTask Save()
    {
        return GameContextManager.Current.DataLoader.SaveEntities(Data.Values, outputPath);
    }
}
