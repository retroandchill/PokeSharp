using System.Diagnostics.CodeAnalysis;

namespace PokeSharp.Core.Data;

public interface IGameDataEntity
{
    static abstract int Count { get; }
}

public interface IGameDataEntity<out TEntity> : IGameDataEntity
    where TEntity : IGameDataEntity
{
    static abstract IEnumerable<TEntity> Entities { get; }
}

public interface IGameDataEntity<TKey, TEntity> : IGameDataEntity<TEntity>
    where TKey : notnull
    where TEntity : IGameDataEntity<TKey, TEntity>
{
    TKey Id { get; }

    static abstract IEnumerable<TKey> Keys { get; }

    static abstract bool Exists(TKey key);

    static abstract TEntity Get(TKey key);

    static abstract bool TryGet(TKey key, [NotNullWhen(true)] out TEntity? entity);
}

public interface IRegisteredGameDataEntity<TEntity> : IGameDataEntity<TEntity>
    where TEntity : IGameDataEntity<TEntity>
{
    static abstract void Register(TEntity entity);
}

public interface IRegisteredGameDataEntity<TKey, TEntity>
    : IGameDataEntity<TKey, TEntity>,
        IRegisteredGameDataEntity<TEntity>
    where TKey : notnull
    where TEntity : IGameDataEntity<TKey, TEntity>;

public interface ILoadedGameDataEntity : IGameDataEntity
{
    static abstract string DataPath { get; }

    static abstract void Load();

    static abstract ValueTask LoadAsync(CancellationToken cancellationToken = default);

    static abstract void Save();

    static abstract ValueTask SaveAsync(CancellationToken cancellationToken = default);
}

public interface ILoadedGameDataEntity<TEntity> : ILoadedGameDataEntity, IGameDataEntity<TEntity>
    where TEntity : IGameDataEntity<TEntity>
{
    static abstract void Import(IEnumerable<TEntity> entities);

    static abstract ValueTask ImportAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    );
}

public interface ILoadedGameDataEntity<TKey, TEntity>
    : IGameDataEntity<TKey, TEntity>,
        ILoadedGameDataEntity<TEntity>
    where TKey : notnull
    where TEntity : IGameDataEntity<TKey, TEntity>;
