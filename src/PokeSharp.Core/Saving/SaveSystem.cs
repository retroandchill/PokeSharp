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

    /// <summary>
    /// Loads and retrieves data from a file located at the specified path.
    /// </summary>
    /// <param name="filePath">The path to the file that contains the data.</param>
    /// <returns>A dictionary containing deserialized data, where keys are of type <see cref="Name"/> and the corresponding values are objects.</returns>
    Dictionary<Name, object> GetDataFromFile(string filePath);

    /// <summary>
    /// Asynchronously retrieves data from a file located at the specified path.
    /// </summary>
    /// <param name="filePath">The path to the file where data is stored.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary of object data indexed by their respective names.</returns>
    ValueTask<Dictionary<Name, object>> GetDataFromFileAsync(
        string filePath,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Saves the provided data to a file at the specified path.
    /// </summary>
    /// <param name="filePath">The path where the data should be saved.</param>
    /// <param name="data">
    /// The data to be saved, represented as a dictionary with keys of type <see cref="Name"/> and
    /// values of type <see cref="object"/>.
    /// </param>
    void SaveToFile(string filePath, Dictionary<Name, object> data);

    /// <summary>
    /// Asynchronously saves the provided data to a file at the specified path.
    /// </summary>
    /// <param name="filePath">The path where the file will be saved.</param>
    /// <param name="data">The data to be saved as key-value pairs.</param>
    /// <param name="cancellationToken">An optional token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    ValueTask SaveToFileAsync(
        string filePath,
        Dictionary<Name, object> data,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a backup of the save data by saving the specified data to the given file path.
    /// </summary>
    /// <param name="filePath">The path where the backup file will be stored.</param>
    /// <param name="data">The data to be backed up, represented as a dictionary of key-value pairs.</param>
    void BackupSaveData(string filePath, Dictionary<Name, object> data);

    /// <summary>
    /// Creates an asynchronous backup of the specified save data to a file.
    /// </summary>
    /// <param name="filePath">The path to the file where the backup will be saved.</param>
    /// <param name="data">The save data to back up, represented as a dictionary of names and objects.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous backup operation.</returns>
    ValueTask BackupSaveDataAsync(
        string filePath,
        Dictionary<Name, object> data,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes the file located at the specified path.
    /// </summary>
    /// <param name="filePath">The path to the file to be deleted.</param>
    void DeleteFile(string filePath);

    /// <summary>
    /// Asynchronously deletes the file located at the specified path.
    /// </summary>
    /// <param name="filePath">The path to the file to be deleted.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    ValueTask DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
}
