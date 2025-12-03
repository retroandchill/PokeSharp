using System.IO.Abstractions;

namespace PokeSharp.Core.Saving;

public sealed class FileSystemSaveReadHandle(IFileSystem fileSystem, string path) : ISaveReadHandle
{
    public Stream Stream { get; } = fileSystem.File.OpenRead(path);

    public void Dispose()
    {
        Stream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Stream.DisposeAsync();
    }
}

public sealed class FileSystemWriteReadHandle : ISaveWriteHandle
{
    private readonly IFileSystem _fileSystem;
    private readonly string _destinationPath;
    private readonly string _tempPath;
    public Stream Stream { get; }

    public FileSystemWriteReadHandle(IFileSystem fileSystem, string path)
    {
        _fileSystem = fileSystem;
        _destinationPath = path;
        _tempPath = $"{path}.tmp";
        Stream = fileSystem.File.OpenWrite(_tempPath);
    }

    public void Commit()
    {
        Stream.Dispose();
        _fileSystem.File.Move(_tempPath, _destinationPath, true);
    }

    public async ValueTask CommitAsync(CancellationToken cancellationToken = default)
    {
        await Stream.DisposeAsync();
        _fileSystem.File.Move(_tempPath, _destinationPath, true);
    }

    public void Dispose()
    {
        Stream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Stream.DisposeAsync();
    }
}
