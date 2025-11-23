using System.IO.Abstractions;
using System.Text;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Core.Utils;

public static partial class FileUtils
{
    [CreateSyncVersion]
    public static async ValueTask WriteFileWithBackupAsync(
        this IFileSystem fileSystem,
        string path,
        Func<StreamWriter, ValueTask> writeAction
    )
    {
        string? backupPath = null;

        if (File.Exists(path))
        {
            backupPath = $"{path}.backup";
            File.Copy(path, backupPath, true);
        }

        try
        {
            await using var fileWriter = fileSystem.File.OpenWrite(path);
            await using var streamWriter = new StreamWriter(fileWriter, Encoding.UTF8);
            await writeAction(streamWriter);
        }
        catch
        {
            if (backupPath is null || !File.Exists(backupPath))
                throw;

            try
            {
                File.Move(backupPath, path, true);
            }
            catch
            {
                File.Delete(path);
                throw;
            }

            throw;
        }
        finally
        {
            // Clean up backup file on success
            if (backupPath != null && File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }
        }
    }
}
