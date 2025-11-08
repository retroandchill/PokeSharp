using System.Diagnostics.CodeAnalysis;

namespace PokeSharp.Core.Data;

public interface IGameDataEntity<TKey, TEntity> where TKey : notnull where TEntity : IGameDataEntity<TKey, TEntity>
{
    public TKey Id { get; }

    public static abstract IEnumerable<TKey> Keys { get; }
    
    public static abstract IEnumerable<TEntity> Entities { get; }
    
    public static abstract int Count { get; }

    public static abstract bool Exists(TKey key);

    public static abstract TEntity Get(TKey key);

    public static abstract bool TryGet(TKey key, [NotNullWhen(true)] out TEntity? entity);
}

public interface IRegisteredGameDataEntity<TKey, TEntity> : IGameDataEntity<TKey, TEntity> where TKey : notnull where TEntity : IGameDataEntity<TKey, TEntity>
{
    public static abstract void Register(TEntity entity);
}

public interface ILoadedGameDataEntity<TKey, TEntity> : IGameDataEntity<TKey, TEntity>
    where TKey : notnull where TEntity : IGameDataEntity<TKey, TEntity>
{
    public static abstract void Load();
    
    public static abstract void Save();
}