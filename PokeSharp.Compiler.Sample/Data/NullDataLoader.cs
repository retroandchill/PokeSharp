using PokeSharp.Core.Data;

namespace PokeSharp.Compiler.Sample.Data;

public class NullDataLoader : IDataLoader
{
    public ValueTask SaveEntities<T>(
        IEnumerable<T> entities,
        string outputPath,
        CancellationToken cancellationToken = default
    )
    {
        return ValueTask.CompletedTask;
    }

    public IAsyncEnumerable<T> LoadEntities<T>(
        string inputPath,
        CancellationToken cancellationToken = default
    )
    {
        return AsyncEnumerable.Empty<T>();
    }
}
