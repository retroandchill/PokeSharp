using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Audio;
using PokeSharp.Core;
using Retro.ReadOnlyParams.Annotations;

namespace PokeSharp.UI;

public class PokemonPauseMenu([ReadOnly] IPokemonPauseMenuScene scene)
{
    public void ShowMenu()
    {
        scene.Refresh();
        scene.ShowMenu();
    }

    private void ShowInfo()
    {
        GameServices.PauseMenuService.ShowInfo(this);
    }

    public async ValueTask StartPokemonMenu(CancellationToken cancellationToken = default)
    {
        scene.StartScene();
        ShowInfo();
        var commandList = new List<Text>();
        var commands = new List<PauseMenuOption>();
        foreach (var (_, handler, name) in GameServices.PauseMenuHandlers.GetAllAvailable())
        {
            commandList.Add(name);
            commands.Add(handler);
        }

        var endScene = false;
        while (true)
        {
            var choice = await scene.ShowCommands(commandList, cancellationToken);
            if (!choice.HasValue)
            {
                GameServices.AudioPlayService.PlayCloseMenuSE();
                endScene = true;
                break;
            }

            if (await commands[choice.Value].Effect(scene, cancellationToken))
            {
                break;
            }
        }

        if (endScene)
        {
            scene.EndScene();
        }
    }
}

public static class PokemonPauseMenuExtensions
{
    private static CachedService<MenuHandlers<PauseMenuOption, NullContext>> _cachedService;

    extension(GameServices)
    {
        public static MenuHandlers<PauseMenuOption, NullContext> PauseMenuHandlers => _cachedService.Instance;
    }
}

public interface IPauseSceneInfoProvider
{
    int Order { get; }

    bool ShowInfo(PokemonPauseMenu pauseMenu);
}

[RegisterSingleton]
[AutoServiceShortcut]
public sealed class PauseMenuService(IEnumerable<IPauseSceneInfoProvider> providers)
{
    private readonly ImmutableArray<IPauseSceneInfoProvider> _providers = [.. providers.OrderBy(x => x.Order)];

    public void ShowInfo(PokemonPauseMenu pauseMenu)
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var provider in _providers)
        {
            if (provider.ShowInfo(pauseMenu))
            {
                return;
            }
        }
    }
}
