namespace PokeSharp.Core.Saving;

/// <summary>
/// Abstraction layer for saving and loading data from files. This is able to be implemented to plug into the save
/// system of the specific game engine being used.
/// </summary>
public interface ISaveSystem
{
    /// <summary>
    /// Checks whether a file exists at the specified path.
    /// </summary>
    /// <param name="filePath">The path to the save file</param>
    /// <returns>Does the file exist?</returns>
    bool Exists(string filePath);

    Stream OpenRead(string filePath);

    Stream OpenWrite(string filePath);

    void Copy(string sourceFilePath, string destinationFilePath);

    ValueTask CopyAsync(
        string sourceFilePath,
        string destinationFilePath,
        CancellationToken cancellationToken = default
    );

    void Delete(string filePath);

    ValueTask DeleteAsync(string filePath, CancellationToken cancellationToken = default);
}
