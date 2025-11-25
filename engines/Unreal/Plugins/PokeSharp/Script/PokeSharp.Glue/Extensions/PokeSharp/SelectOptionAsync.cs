using UnrealSharp.PokeSharpCore;
using UnrealSharp.UnrealSharpAsync;

// ReSharper disable once CheckNamespace
namespace UnrealSharp.PokeSharp;

internal partial class USelectOptionAsync
{
    private TaskCompletionSource<TOptional<int>> _tcs = new();

    private readonly Action _onAsyncCompleted;

    internal USelectOptionAsync()
    {
        _onAsyncCompleted = OnAsyncCompleted;
    }

    private void OnAsyncCompleted()
    {
        _tcs.SetResult(SelectedIndex);
    }

    public override void Dispose()
    {
        base.Dispose();
        AsyncLoadUtilities.DisposeAsyncLoadTask(ref _tcs);
    }

    public static Task<TOptional<int>> SelectOptionAsync(
        USelectableWidget selectableWidget,
        CancellationToken cancellationToken = default
    )
    {
        var async = NewObject<USelectOptionAsync>(selectableWidget);
        NativeAsyncUtilities.InitializeAsyncAction(async, async._onAsyncCompleted);
        async.SelectOption(selectableWidget);
        cancellationToken.Register(selectableWidget.DeactivateWidget);
        return async._tcs.Task;
    }
}
