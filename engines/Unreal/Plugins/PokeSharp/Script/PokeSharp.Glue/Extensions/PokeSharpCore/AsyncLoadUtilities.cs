using UnrealSharp.Core;
using UnrealSharp.CoreUObject;
using UnrealSharp.Engine;
using UnrealSharp.UnrealSharpAsync;

// ReSharper disable once CheckNamespace
namespace UnrealSharp.PokeSharpCore;

internal static class AsyncLoadUtilities
{
    internal static UObject WorldContextObject
    {
        get
        {
            var worldContextObject = FCSManagerExporter.CallGetCurrentWorldContext();
            var worldContextHandle = FCSManagerExporter.CallFindManagedObject(worldContextObject);
            return GCHandleUtilities.GetObjectFromHandlePtr<UObject>(worldContextHandle)!;
        }
    }

    internal static void DisposeAsyncLoadTask<T>(ref TaskCompletionSource<T> tcs)
    {
        if (!tcs.Task.IsCompleted)
        {
            tcs.SetCanceled();
        }

        tcs.Task.Dispose();
        tcs = null!;
    }
}

internal partial class ULoadGameFromSlotAsync
{
    public Task<USaveGame> Task => _tcs.Task;
    private TaskCompletionSource<USaveGame> _tcs = new();
    private readonly Action _action;

    public ULoadGameFromSlotAsync()
    {
        _action = OnAsyncCompleted;
    }

    public static Task<USaveGame> LoadGameFromSlotAsync(string slotName, int userIndex)
    {
        var loader = NewObject<ULoadGameFromSlotAsync>(AsyncLoadUtilities.WorldContextObject);

        NativeAsyncUtilities.InitializeAsyncAction(loader, loader._action);
        loader.LoadGameFromSlot(slotName, userIndex);
        return loader.Task;
    }

    public override void Dispose()
    {
        base.Dispose();
        AsyncLoadUtilities.DisposeAsyncLoadTask(ref _tcs);
    }

    private void OnAsyncCompleted()
    {
        _tcs.TrySetResult(SaveGame);
    }
}

internal partial class USaveGameToSlotAsync
{
    public Task<bool> Task => _tcs.Task;
    private TaskCompletionSource<bool> _tcs = new();
    private readonly Action _action;

    public USaveGameToSlotAsync()
    {
        _action = OnAsyncCompleted;
    }

    public static Task<bool> SaveGameToSlotAsync(USaveGame saveGame, string slotName, int userIndex)
    {
        var saver = NewObject<USaveGameToSlotAsync>(AsyncLoadUtilities.WorldContextObject);
        saver.SaveGameToSlot(slotName, userIndex, saveGame);
        NativeAsyncUtilities.InitializeAsyncAction(saver, saver._action);
        return saver.Task;
    }

    public override void Dispose()
    {
        base.Dispose();
        AsyncLoadUtilities.DisposeAsyncLoadTask(ref _tcs);
    }

    private void OnAsyncCompleted()
    {
        _tcs.TrySetResult(Succeeded);
    }
}

public static class SaveGameExtensions
{
    extension(UGameplayStatics)
    {
        public static Task<USaveGame> LoadGameFromSlotAsync(string slotName, int userIndex)
        {
            return ULoadGameFromSlotAsync.LoadGameFromSlotAsync(slotName, userIndex);
        }

        public static Task<bool> SaveGameToSlotAsync(USaveGame saveGame, string slotName, int userIndex)
        {
            return USaveGameToSlotAsync.SaveGameToSlotAsync(saveGame, slotName, userIndex);
        }
    }
}
