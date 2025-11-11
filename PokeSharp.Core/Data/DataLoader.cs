namespace PokeSharp.Core.Data;

public interface IDataLoader
{
    void SaveEntities<T>(IEnumerable<T> entities, string outputPath);

    ValueTask SaveEntitiesAsync<T>(
        IEnumerable<T> entities,
        string outputPath,
        CancellationToken cancellationToken = default
    );

    IEnumerable<T> LoadEntities<T>(string inputPath);

    IAsyncEnumerable<T> LoadEntitiesAsync<T>(
        string inputPath,
        CancellationToken cancellationToken = default
    );
}
