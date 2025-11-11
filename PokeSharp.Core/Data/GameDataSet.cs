using System.Diagnostics.CodeAnalysis;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Core.Data;

public abstract partial class GameDataSet<TEntity, TKey>
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

    [CreateSyncVersion]
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

[RegisterSingleton]
public sealed class RegisteredGameDataSet<TEntity, TKey> : GameDataSet<TEntity, TKey>
    where TEntity : IRegisteredGameDataEntity<TKey, TEntity>
    where TKey : notnull
{
    public void Register(TEntity entity)
    {
        Data.Add(entity.Id, entity);
    }
}

[RegisterSingleton]
public sealed partial class LoadedGameDataSet<TEntity, TKey> : GameDataSet<TEntity, TKey>
    where TEntity : ILoadedGameDataEntity<TKey, TEntity>
    where TKey : notnull
{
    [CreateSyncVersion]
    public async ValueTask ImportAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    )
    {
        ReplaceData(entities);
        await SaveAsync(cancellationToken);
    }

    [CreateSyncVersion]
    public ValueTask LoadAsync(CancellationToken cancellationToken = default)
    {
        return ReplaceDataAsync(
            GameContextManager.Current.DataLoader.LoadEntitiesAsync<TEntity>(
                TEntity.DataPath,
                cancellationToken
            )
        );
    }

    [CreateSyncVersion]
    public ValueTask SaveAsync(CancellationToken cancellationToken = default)
    {
        return GameContextManager.Current.DataLoader.SaveEntitiesAsync(
            Data.Values,
            TEntity.DataPath,
            cancellationToken
        );
    }
}
