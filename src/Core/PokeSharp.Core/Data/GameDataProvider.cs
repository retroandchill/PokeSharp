namespace PokeSharp.Core.Data;

/// <summary>
/// Provider used to register static game data.
/// </summary>
/// <typeparam name="T">The type of entity to register</typeparam>
public interface IGameDataProvider<out T>
    where T : IRegisteredGameDataEntity
{
    /// <summary>
    /// The priority of this provider, lower priority providers are invoked first.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Gets the list of new entities to register.
    /// </summary>
    /// <returns>An enumerable list of entities to register.</returns>
    IEnumerable<T> GetEntitiesToRegister();
}
