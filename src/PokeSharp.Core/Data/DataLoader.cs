namespace PokeSharp.Core.Data;

/// <summary>
/// Represents an interface that facilitates the loading and saving
/// of entity data in a customizable manner.
/// </summary>
public interface IDataLoader
{
    /// <summary>
    /// Saves a collection of entities to the specified output path.
    /// </summary>
    /// <typeparam name="T">The type of the entities to save.</typeparam>
    /// <param name="entities">The collection of entities to be saved.</param>
    /// <param name="outputPath">The path to save the entities to.</param>
    void SaveEntities<T>(IEnumerable<T> entities, string outputPath);

    /// <summary>
    /// Asynchronously saves a collection of entities to the specified output path.
    /// </summary>
    /// <typeparam name="T">The type of the entities to save.</typeparam>
    /// <param name="entities">The collection of entities to be saved.</param>
    /// <param name="outputPath">The path to save the entities to.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    ValueTask SaveEntitiesAsync<T>(
        IEnumerable<T> entities,
        string outputPath,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Loads a collection of entities from the specified input path.
    /// </summary>
    /// <typeparam name="T">The type of the entities to load.</typeparam>
    /// <param name="inputPath">The path from which the entities are loaded.</param>
    /// <returns>A collection of entities of the specified type.</returns>
    IEnumerable<T> LoadEntities<T>(string inputPath);

    /// <summary>
    /// Asynchronously loads a collection of entities from the specified input path.
    /// </summary>
    /// <typeparam name="T">The type of the entities to load.</typeparam>
    /// <param name="inputPath">The path to load the entities from.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An asynchronous enumerable containing the loaded entities.</returns>
    IAsyncEnumerable<T> LoadEntitiesAsync<T>(string inputPath, CancellationToken cancellationToken = default);
}
