using PokeSharp.Core.Saving;
using UnrealSharp.Engine;
using UnrealSharp.PokeSharpCore;

namespace PokeSharp.Unreal.Core.Saving;

public sealed class UnrealSaveReadHandle(UPokeSharpSaveGame saveGame) : ISaveReadHandle
{
    public Stream Stream { get; } = new UnrealSaveStream(saveGame, false);

    public void Dispose()
    {
        Stream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Stream.DisposeAsync();
    }
}

public sealed class UnrealSaveWriteHandle(UPokeSharpSaveGame saveGame, string slotName, int userIndex)
    : ISaveWriteHandle
{
    public Stream Stream { get; } = new UnrealSaveStream(saveGame, true);

    public void Commit()
    {
        UGameplayStatics.SaveGameToSlot(saveGame, slotName, userIndex);
    }

    public async ValueTask CommitAsync(CancellationToken cancellationToken = default)
    {
        await UGameplayStatics.SaveGameToSlotAsync(saveGame, slotName, userIndex);
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
