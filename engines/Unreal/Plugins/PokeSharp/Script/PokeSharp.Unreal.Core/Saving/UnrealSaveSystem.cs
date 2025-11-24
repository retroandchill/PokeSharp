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

    public ISaveReadHandle OpenRead(string filePath)
    {
        var saveData = UGameplayStatics.LoadGameFromSlot(filePath, UserIndex);
        return saveData is UPokeSharpSaveGame pokeSharpSaveGame
            ? new UnrealSaveReadHandle(pokeSharpSaveGame)
            : throw new InvalidOperationException("Save data is not a PokeSharp save game");
    }

    public async ValueTask<ISaveReadHandle> OpenReadAsync(
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        var saveData = await UGameplayStatics.LoadGameFromSlotAsync(filePath, UserIndex);
        return saveData is UPokeSharpSaveGame pokeSharpSaveGame
            ? new UnrealSaveReadHandle(pokeSharpSaveGame)
            : throw new InvalidOperationException("Save data is not a PokeSharp save game");
    }

    public ISaveWriteHandle OpenWrite(string filePath)
    {
        var saveData = UPokeSharpSaveGame.CreateSaveGame();
        return new UnrealSaveWriteHandle(saveData, filePath, UserIndex);
    }

    public ValueTask<ISaveWriteHandle> OpenWriteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(OpenWrite(filePath));
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
