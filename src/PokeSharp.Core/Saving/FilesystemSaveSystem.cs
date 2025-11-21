using Microsoft.Extensions.Options;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Core.Saving;

[RegisterSingleton]
public partial class FilesystemSaveSystem(IOptionsMonitor<SaveDataConfig> config) : ISaveSystem
{
    public bool Exists(string filePath)
    {
        return File.Exists(Path.Join(config.CurrentValue.SaveFilePath, filePath));
    }

    public Stream OpenRead(string filePath)
    {
        return File.OpenRead(Path.Join(config.CurrentValue.SaveFilePath, filePath));
    }

    public Stream OpenWrite(string filePath)
    {
        if (!Directory.Exists(config.CurrentValue.SaveFilePath))
        {
            Directory.CreateDirectory(config.CurrentValue.SaveFilePath);
        }

        return File.OpenWrite(Path.Join(config.CurrentValue.SaveFilePath, filePath));
    }

    [CreateSyncVersion]
    public ValueTask CopyAsync(
        string sourceFilePath,
        string destinationFilePath,
        CancellationToken cancellationToken = default
    )
    {
        File.Copy(
            Path.Join(config.CurrentValue.SaveFilePath, sourceFilePath),
            Path.Join(config.CurrentValue.SaveFilePath, destinationFilePath)
        );
        return ValueTask.CompletedTask;
    }

    [CreateSyncVersion]
    public ValueTask DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        File.Delete(Path.Join(config.CurrentValue.SaveFilePath, filePath));
        return ValueTask.CompletedTask;
    }
}
