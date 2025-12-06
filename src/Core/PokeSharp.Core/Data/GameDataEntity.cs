using System.Diagnostics.CodeAnalysis;
using PokeSharp.Core.Strings;

namespace PokeSharp.Core.Data;

/// <summary>
/// Represents a basic interface for game data entities.
/// Defines essential properties or methods common to all game data entities.
/// </summary>
public interface IGameDataEntity
{
    /// <summary>
    /// Gets the total number of entities contained within the current data set.
    /// Represents the count of elements stored in the underlying data structure.
    /// </summary>
    static abstract int Count { get; }
}

public interface INamedGameDataEntity : IGameDataEntity
{
    Text Name { get; }
}

/// <summary>
/// Represents a basic interface for game data entities.
/// Defines shared properties or methods common to game data entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IGameDataEntity<out TEntity> : IGameDataEntity
    where TEntity : IGameDataEntity
{
    /// <summary>
    /// Provides access to a collection of all entities of the specified type within the data set.
    /// Represents a centralized collection for querying or processing game data entities.
    /// </summary>
    static abstract IEnumerable<TEntity> Entities { get; }
}

/// <summary>
/// Defines the foundation for game data entities, serving as a marker interface for general game data types.
/// Commonly implemented to ensure type consistency across game data systems.
/// </summary>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IGameDataEntity<TKey, TEntity> : IGameDataEntity<TEntity>
    where TKey : notnull
    where TEntity : IGameDataEntity<TKey, TEntity>
{
    /// <summary>
    /// Gets the unique identifier for the current entity.
    /// Represents a distinct key used to differentiate this entity within its data set or database.
    /// </summary>
    TKey Id { get; }

    /// <summary>
    /// Represents the collection of unique identifiers associated with the entities in the data set.
    /// Provides access to all keys used for identifying individual game data entities.
    /// </summary>
    static abstract IEnumerable<TKey> Keys { get; }

    /// <summary>
    /// Determines whether an entity with the specified key exists in the data set.
    /// </summary>
    /// <param name="key">The key representing the entity to check for existence.</param>
    /// <returns>True if an entity with the specified key exists; otherwise, false.</returns>
    static abstract bool Exists(TKey key);

    /// <summary>
    /// Retrieves the entity associated with the specified key.
    /// </summary>
    /// <param name="key">The key identifying the entity to retrieve.</param>
    /// <returns>The entity associated with the specified key.</returns>
    static abstract TEntity Get(TKey key);

    /// <summary>
    /// Attempts to retrieve the entity associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the entity to retrieve.</param>
    /// <param name="entity">
    /// When this method returns, contains the entity associated with the specified key,
    /// if the key is found; otherwise, null. This parameter is passed uninitialized.
    /// </param>
    /// <returns>True if the entity with the specified key exists and was retrieved successfully; otherwise, false.</returns>
    static abstract bool TryGet(TKey key, [NotNullWhen(true)] out TEntity? entity);

    static abstract int IndexOf(TKey key);
}

/// <summary>
/// Represents an interface for entities that are registered as part of game data.
/// Provides functionality for managing the registration of game data entities.
/// </summary>
public interface IRegisteredGameDataEntity;

/// <summary>
/// Represents an interface for entities that are registered as part of game data.
/// Provides functionality for managing the registration of game data entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being registered.</typeparam>
public interface IRegisteredGameDataEntity<out TEntity> : IGameDataEntity<TEntity>, IRegisteredGameDataEntity
    where TEntity : IGameDataEntity<TEntity>;

/// <summary>
/// Represents an interface for registered game data entities.
/// Combines functionality for identifying and managing game data entities within a registered state,
/// while extending support for key-based and entity-specific operations.
/// </summary>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
/// <typeparam name="TEntity">The type of the entity being registered.</typeparam>
public interface IRegisteredGameDataEntity<TKey, TEntity>
    : IGameDataEntity<TKey, TEntity>,
        IRegisteredGameDataEntity<TEntity>
    where TKey : notnull
    where TEntity : IGameDataEntity<TKey, TEntity>;

/// <summary>
/// Represents an interface for game data entities that can be loaded and saved.
/// Provides functionality for loading and saving data, as well as asynchronous operations.
/// </summary>
public interface ILoadedGameDataEntity : IGameDataEntity
{
    /// <summary>
    /// Gets the file system path where the data associated with the corresponding game entity is stored.
    /// Represents a standardized and abstract location for loading and saving entity data.
    /// </summary>
    static abstract string DataPath { get; }

    static virtual bool IsOptional => false;

    /// <summary>
    /// Loads game data from the specified source.
    /// </summary>
    static abstract void Load();

    /// <summary>
    /// Asynchronously loads game data from the specified source.
    /// </summary>
    /// <param name="cancellationToken">The token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    static abstract ValueTask LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the current state of the game data to the specified destination.
    /// </summary>
    static abstract void Save();

    /// <summary>
    /// Asynchronously saves the current state of game data to the persistent storage.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    static abstract ValueTask SaveAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an interface for game data entities that can be loaded and are associated with a specific data path.
/// Defines static members for loading data and obtaining the data path.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being loaded.</typeparam>
public interface ILoadedGameDataEntity<TEntity> : ILoadedGameDataEntity, IGameDataEntity<TEntity>
    where TEntity : IGameDataEntity<TEntity>
{
    /// <summary>
    /// Imports a collection of entities into the game data system.
    /// </summary>
    /// <param name="entities">The collection of entities to be imported.</param>
    static abstract void Import(IEnumerable<TEntity> entities);

    /// <summary>
    /// Asynchronously imports a collection of entities into the system.
    /// </summary>
    /// <param name="entities">The collection of entities to be imported.</param>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    static abstract ValueTask ImportAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an interface for game data entities that are loaded and accessible within the application.
/// This interface extends the base game data entity functionality for entities that have been initialized or loaded into the system.
/// </summary>
public interface ILoadedGameDataEntity<TKey, TEntity> : IGameDataEntity<TKey, TEntity>, ILoadedGameDataEntity<TEntity>
    where TKey : notnull
    where TEntity : IGameDataEntity<TKey, TEntity>;
