namespace PokeSharp.Core.Data;

public interface IDataLoader
{
    ValueTask SaveEntities<T>(IEnumerable<T> entities, string outputPath);

    IAsyncEnumerable<T> LoadEntities<T>(
        string inputPath,
        CancellationToken cancellationToken = default
    );
}
