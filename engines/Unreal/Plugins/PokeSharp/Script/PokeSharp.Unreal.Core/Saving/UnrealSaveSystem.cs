using PokeSharp.Core.Saving;
using UnrealSharp;
using UnrealSharp.Engine;
using UnrealSharp.PokeSharpCore;

namespace PokeSharp.Unreal.Core.Saving;

[RegisterSingleton(Duplicate = DuplicateStrategy.Replace)]
public class UnrealSaveSystem : ISaveSystem
{
    private const int UserIndex = 0;

    public bool Exists(string filePath)
    {
        return UGameplayStatics.DoesSaveGameExist(filePath, UserIndex);
    }

    public Stream OpenRead(string filePath)
    {
        return new UnrealSaveStream(filePath, UserIndex);
    }

    public Stream OpenWrite(string filePath)
    {
        var saveData = UPokeSharpSaveGame.CreateSaveGame();
        return new UnrealSaveStream(saveData, true);
    }

    public void Copy(string sourceFilePath, string destinationFilePath)
    {
        var saveData = UGameplayStatics.LoadGameFromSlot(sourceFilePath, UserIndex);
        UGameplayStatics.SaveGameToSlot(saveData, destinationFilePath, UserIndex);
    }

    public async ValueTask CopyAsync(
        string sourceFilePath,
        string destinationFilePath,
        CancellationToken cancellationToken = default
    )
    {
        var saveData = await UGameplayStatics
            .LoadGameFromSlotAsync(sourceFilePath, UserIndex)
            .ConfigureWithUnrealContext();
        await UGameplayStatics
            .SaveGameToSlotAsync(saveData, destinationFilePath, UserIndex)
            .ConfigureWithUnrealContext();
    }

    public void Delete(string filePath)
    {
        UGameplayStatics.DeleteGameInSlot(filePath, UserIndex);
    }

    public ValueTask DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        Delete(filePath);
        return ValueTask.CompletedTask;
    }
}
