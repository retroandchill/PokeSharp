using System.Text;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Core.Utils;

public static partial class FileUtils
{
    [CreateSyncVersion]
    public static async ValueTask WriteFileWithBackupAsync(
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
            await using var fileWriter = new StreamWriter(path, false, Encoding.UTF8);
            await writeAction(fileWriter);
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
