namespace PokeSharp.Core.Saving;

public interface ISaveReadHandle : IDisposable, IAsyncDisposable
{
    Stream Stream { get; }
}

public interface ISaveWriteHandle : IDisposable, IAsyncDisposable
{
    Stream Stream { get; }

    void Commit();

    ValueTask CommitAsync(CancellationToken cancellationToken = default);
}
