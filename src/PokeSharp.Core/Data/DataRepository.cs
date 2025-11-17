namespace PokeSharp.Core.Data;

public interface IDataRepository
{
    /// <summary>
    /// Loads game data from the specified source.
    /// </summary>
    void Load();

    /// <summary>
    /// Asynchronously loads game data from the specified source.
    /// </summary>
    /// <param name="cancellationToken">The token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask LoadAsync(CancellationToken cancellationToken = default);
}
