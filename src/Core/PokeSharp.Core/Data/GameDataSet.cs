using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core.Collections.Immutable;
using Retro.ReadOnlyParams.Annotations;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Core.Data;

/// <summary>
/// Represents a collection of game data entities organized by unique keys.
/// </summary>
/// <typeparam name="TEntity">The type of the entities stored in this data set.</typeparam>
/// <typeparam name="TKey">The type of the key used to identify entities.</typeparam>
public abstract class GameDataSet<TEntity, TKey>
    where TEntity : IGameDataEntity<TKey, TEntity>
    where TKey : notnull
{
    private ImmutableOrderedDictionary<TKey, TEntity> _data = new();

    /// <summary>
    /// Gets the internal collection of game data entities organized as an ordered dictionary.
    /// Provides the ability to access, manipulate, and manage the stored game data.
    /// </summary>
    /// <remarks>
    /// This property is protected and provides access to the underlying ordered dictionary
    /// that represents the dataset of entities. It is used within the class and subclasses
    /// to perform operations like adding or retrieving entities by their unique keys.
    /// </remarks>
    public ImmutableOrderedDictionary<TKey, TEntity> Data => _data;

    /// <summary>
    /// Provides a collection of all unique keys associated with the game data entities in the dataset.
    /// Enables enumeration and retrieval of keys for identification and operation purposes.
    /// </summary>
    /// <remarks>
    /// This property is public and exposes the keys present in the underlying collection.
    /// It allows for iteration over the keys and serves as a reference for identifying entities
    /// within the dataset by their respective unique keys.
    /// </remarks>
    public IEnumerable<TKey> Keys => _data.Keys;

    /// <summary>
    /// Provides access to the collection of entities stored in the dataset.
    /// </summary>
    /// <remarks>
    /// This property offers an enumerable sequence of all entities contained within the dataset, allowing iteration
    /// over the values managed by the underlying ordered dictionary. It simplifies access to the stored data
    /// without needing direct interaction with the internal data structure.
    /// </remarks>
    public IEnumerable<TEntity> Entities => _data.Values;

    /// <summary>
    /// Gets the total number of entities currently stored in the dataset.
    /// </summary>
    /// <remarks>
    /// This property provides the count of elements within the internal data collection, reflecting
    /// the number of key-entity pairs stored in the dataset. It is a read-only property and updates
    /// dynamically as entities are added or removed from the dataset.
    /// </remarks>
    public int Count => _data.Count;

    /// <summary>
    /// Determines whether an entity with the specified key exists in the data set.
    /// </summary>
    /// <param name="key">The key of the entity to locate in the data set.</param>
    /// <returns>True if an entity with the specified key exists; otherwise, false.</returns>
    public bool Exists(TKey key) => _data.ContainsKey(key);

    /// <summary>
    /// Retrieves an entity associated with the specified key from the data set.
    /// </summary>
    /// <param name="key">The key of the entity to retrieve.</param>
    /// <returns>The entity associated with the specified key.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no entity with the specified key is found.</exception>
    public TEntity Get(TKey key)
    {
        return _data.TryGetValue(key, out var entity)
            ? entity
            : throw new KeyNotFoundException($"Could not find entity with key {key}");
    }

    /// <summary>
    /// Attempts to retrieve an entity with the specified key from the data set.
    /// </summary>
    /// <param name="key">The key of the entity to locate in the data set.</param>
    /// <param name="entity">
    /// When this method returns, contains the entity associated with the specified key,
    /// if it is found in the data set; otherwise, null. This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// True if an entity with the specified key is found in the data set; otherwise, false.
    /// </returns>
    public bool TryGet(TKey key, [NotNullWhen(true)] out TEntity? entity) => _data.TryGetValue(key, out entity);

    public int IndexOf(TKey key)
    {
        return _data.IndexOf(key);
    }

    protected void ReplaceData(ImmutableOrderedDictionary<TKey, TEntity> newData)
    {
        Interlocked.Exchange(ref _data, newData);
    }

    protected void ReplaceData(IEnumerable<TEntity> entities)
    {
        ReplaceData(entities.ToImmutableOrderedDictionary(x => x.Id));
    }

    /// <summary>
    /// Replaces the current data set with a new set of entities provided asynchronously.
    /// </summary>
    /// <param name="entities">The asynchronous enumerable of entities to replace the current data set with.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if required.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    protected async ValueTask ReplaceDataAsync(
        IAsyncEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    )
    {
        var newData = ImmutableOrderedDictionary.CreateBuilder<TKey, TEntity>();
        await foreach (var entity in entities.WithCancellation(cancellationToken))
        {
            newData.Add(entity.Id, entity);
        }

        ReplaceData(newData.ToImmutable());
    }
}

/// <summary>
/// Represents a specialized data set for managing and registering game data entities.
/// This class extends the base functionality of <see cref="GameDataSet{TEntity, TKey}"/> to include
/// registration capabilities for entities that implement <see cref="IRegisteredGameDataEntity{TKey, TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">
/// The type of entities stored in this data set. Must implement <see cref="IRegisteredGameDataEntity{TKey, TEntity}"/>.
/// </typeparam>
/// <typeparam name="TKey">
/// The type of the key used to uniquely identify entities in the data set.
/// </typeparam>
[RegisterSingleton]
public sealed class RegisteredGameDataSet<TEntity, TKey> : GameDataSet<TEntity, TKey>
    where TEntity : IRegisteredGameDataEntity<TKey, TEntity>
    where TKey : notnull
{
    /// <summary>
    /// Constructs a new instance of the <see cref="RegisteredGameDataSet{TEntity, TKey}"/> class.
    /// </summary>
    /// <param name="providers">Services that will register all the static data.</param>
    public RegisteredGameDataSet(IEnumerable<IGameDataProvider<TEntity>> providers)
    {
        foreach (var entity in providers.OrderBy(p => p.Priority).SelectMany(p => p.GetEntitiesToRegister()))
        {
            Data.Add(entity.Id, entity);
        }
    }
}

public interface ILoadedGameDataSet
{
    string DataPath { get; }

    bool IsOptional { get; }

    void Load();

    ValueTask LoadAsync(CancellationToken cancellationToken = default);

    void Save();

    ValueTask SaveAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a specialized data set for managing game data entities that are loaded dynamically.
/// </summary>
/// <typeparam name="TEntity">The type of the loaded game data entities stored in this data set.</typeparam>
/// <typeparam name="TKey">The type of the key used to uniquely identify entities.</typeparam>
[RegisterSingleton(ServiceType = typeof(LoadedGameDataSet<,>))]
public sealed partial class LoadedGameDataSet<TEntity, TKey>([ReadOnly] IDataLoader dataLoader)
    : GameDataSet<TEntity, TKey>,
        ILoadedGameDataSet
    where TEntity : ILoadedGameDataEntity<TKey, TEntity>
    where TKey : notnull
{
    public string DataPath => TEntity.DataPath;

    public bool IsOptional => TEntity.IsOptional;

    /// <summary>
    /// Imports a collection of entities into the data set asynchronously.
    /// </summary>
    /// <param name="entities">The collection of entities to import into the data set.</param>
    /// <param name="shouldSave">Whether the changes should be saved to the storage.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if required.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [CreateSyncVersion]
    public async ValueTask ImportAsync(
        IEnumerable<TEntity> entities,
        bool shouldSave = true,
        CancellationToken cancellationToken = default
    )
    {
        ReplaceData(entities);
        if (shouldSave)
        {
            await SaveAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Imports a collection of entities into the data set asynchronously.
    /// </summary>
    /// <param name="entities">The collection of entities to import into the data set.</param>
    /// <param name="shouldSave">Whether the changes should be saved to the storage.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if required.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [CreateSyncVersion]
    public async ValueTask ImportAsync(
        ImmutableOrderedDictionary<TKey, TEntity> entities,
        bool shouldSave = true,
        CancellationToken cancellationToken = default
    )
    {
        ReplaceData(entities);
        if (shouldSave)
        {
            await SaveAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Asynchronously loads the game data entities into the data set.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [CreateSyncVersion]
    public ValueTask LoadAsync(CancellationToken cancellationToken = default)
    {
        return ReplaceDataAsync(
            dataLoader.LoadEntitiesAsync<TEntity>(TEntity.DataPath, cancellationToken),
            cancellationToken
        );
    }

    /// <summary>
    /// Asynchronously saves the current collection of entities to the storage.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    [CreateSyncVersion]
    public ValueTask SaveAsync(CancellationToken cancellationToken = default)
    {
        return dataLoader.SaveEntitiesAsync(Data.Values, TEntity.DataPath, cancellationToken);
    }
}

public static class GameDataSetExtensions
{
    public static IServiceCollection RegisterGameDataRepository<TEntity, TKey>(this IServiceCollection services)
        where TEntity : ILoadedGameDataEntity<TKey, TEntity>
        where TKey : notnull
    {
        return services.AddSingleton<ILoadedGameDataSet>(sp =>
            sp.GetRequiredService<LoadedGameDataSet<TEntity, TKey>>()
        );
    }
}
