using System.Diagnostics.CodeAnalysis;
using PokeSharp.Core.Strings;
using PokeSharp.UI.Pause;
using PokeSharp.Unreal.Core.Strings;
using PokeSharp.Unreal.Settings;
using UnrealSharp;
using UnrealSharp.CoreUObject;
using UnrealSharp.GameplayTags;
using UnrealSharp.PokeSharp;
using UnrealSharp.PokeSharpCore;

namespace PokeSharp.Unreal.UI.Pause;

public class UnrealPokemonPauseMenuScene : IPokemonPauseMenuScene
{
    private static readonly FGameplayTag LayerTag = new("UI.Layer.Menu");
    private UPauseMenuWidget? _pauseMenuWidget;

    public async ValueTask StartScene(CancellationToken cancellationToken = default)
    {
        if (_pauseMenuWidget is not null)
        {
            throw new InvalidOperationException("Scene already started");
        }

        var primaryPlayer = UCommonUIExtensions.PrimaryPlayerController;
        _pauseMenuWidget = await primaryPlayer.PushContentToLayerAsync(
            LayerTag,
            UObject.GetDefault<UMenuWidgetSettings>().PauseMenuWidgetClass,
            cancellationToken: cancellationToken
        );
    }

    public void EndScene()
    {
        ValidateSceneStarted();
        _pauseMenuWidget.PopContentFromLayer();
    }

    public void ShowInfo(Text text)
    {
        // TODO: Add the help window for showing info
    }

    public void ShowMenu()
    {
        // No-op as the layering system handles this
    }

    public void HideMenu()
    {
        // No-op as the layering system handles this
    }

    public async ValueTask<int?> ShowCommands(
        IReadOnlyList<Text> handlers,
        CancellationToken cancellationToken = default
    )
    {
        ValidateSceneStarted();
        _pauseMenuWidget.Commands = handlers.Select(x => new FCommandData(x.ToUnrealText())).ToArray();
        var selectedOption = await _pauseMenuWidget.SelectOptionAsync(cancellationToken);
        return selectedOption.ToNullable();
    }

    public void Refresh()
    {
        // No-op as there's nothing that needs to be refreshed
    }

    [MemberNotNull(nameof(_pauseMenuWidget))]
    private void ValidateSceneStarted()
    {
        if (_pauseMenuWidget is null)
        {
            throw new InvalidOperationException("Scene not started");
        }
    }
}
