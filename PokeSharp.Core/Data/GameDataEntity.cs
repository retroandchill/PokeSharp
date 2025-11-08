using System.Diagnostics.CodeAnalysis;

namespace PokeSharp.Core.Data;

public interface IGameDataEntity
{
    public static abstract int Count { get; }
}

public interface IGameDataEntity<out TEntity> : IGameDataEntity where TEntity : IGameDataEntity
{
    public static abstract IEnumerable<TEntity> Entities { get; }
}

public interface IGameDataEntity<TKey, TEntity> : IGameDataEntity<TEntity> where TKey : notnull where TEntity : IGameDataEntity<TKey, TEntity>
{
    public TKey Id { get; }

    public static abstract IEnumerable<TKey> Keys { get; }

    public static abstract bool Exists(TKey key);

    public static abstract TEntity Get(TKey key);

    public static abstract bool TryGet(TKey key, [NotNullWhen(true)] out TEntity? entity);
}

public interface IRegisteredGameDataEntity<TEntity> : IGameDataEntity<TEntity> where TEntity : IGameDataEntity<TEntity>
{
    public static abstract void Register(TEntity entity);
}

public interface IRegisteredGameDataEntity<TKey, TEntity> : IGameDataEntity<TKey, TEntity>, IRegisteredGameDataEntity<TEntity>
    where TKey : notnull where TEntity : IGameDataEntity<TKey, TEntity>;

public interface ILoadedGameDataEntity : IGameDataEntity
{
    public static abstract void Load();
    
    public static abstract void Save();
}

public interface ILoadedGameDataEntity<in TEntity> : IGameDataEntity
{
    public static abstract void Import(IEnumerable<TEntity> entities);
}

public interface ILoadedGameDataEntity<TKey, TEntity> : IGameDataEntity<TKey, TEntity>, ILoadedGameDataEntity<TEntity>
    where TKey : notnull where TEntity : IGameDataEntity<TKey, TEntity>;