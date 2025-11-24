using System.IO.Abstractions;
using Microsoft.Extensions.Options;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Core.Saving;

[RegisterSingleton]
public partial class FilesystemSaveSystem(IFileSystem fileSystem, IOptionsMonitor<SaveDataConfig> config) : ISaveSystem
{
    public bool Exists(string filePath)
    {
        return fileSystem.File.Exists(Path.Join(config.CurrentValue.SaveFilePath, filePath));
    }

    public ISaveReadHandle OpenRead(string filePath)
    {
        return new FileSystemSaveReadHandle(
            fileSystem,
            fileSystem.Path.Join(config.CurrentValue.SaveFilePath, filePath)
        );
    }

    public ValueTask<ISaveReadHandle> OpenReadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(OpenRead(filePath));
    }

    public ISaveWriteHandle OpenWrite(string filePath)
    {
        if (!fileSystem.Directory.Exists(config.CurrentValue.SaveFilePath))
        {
            fileSystem.Directory.CreateDirectory(config.CurrentValue.SaveFilePath);
        }

        return new FileSystemWriteReadHandle(
            fileSystem,
            fileSystem.Path.Join(config.CurrentValue.SaveFilePath, filePath)
        );
    }

    public ValueTask<ISaveWriteHandle> OpenWriteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(OpenWrite(filePath));
    }

    [CreateSyncVersion]
    public ValueTask CopyAsync(
        string sourceFilePath,
        string destinationFilePath,
        CancellationToken cancellationToken = default
    )
    {
        fileSystem.File.Copy(
            fileSystem.Path.Join(config.CurrentValue.SaveFilePath, sourceFilePath),
            fileSystem.Path.Join(config.CurrentValue.SaveFilePath, destinationFilePath)
        );
        return ValueTask.CompletedTask;
    }

    [CreateSyncVersion]
    public ValueTask DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        fileSystem.File.Delete(fileSystem.Path.Join(config.CurrentValue.SaveFilePath, filePath));
        return ValueTask.CompletedTask;
    }
}
