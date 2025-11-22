using PokeSharp.Audio;
using PokeSharp.Core;
using Retro.ReadOnlyParams.Annotations;

namespace PokeSharp.UI.Pause;

public class PokemonPauseScreen([ReadOnly] IPokemonPauseMenuScene scene) : IScreen
{
    public void ShowMenu()
    {
        scene.Refresh();
        scene.ShowMenu();
    }

    private void ShowInfo()
    {
        GameGlobal.PauseMenuService.ShowInfo(this);
    }

    public async ValueTask StartPokemonMenu(CancellationToken cancellationToken = default)
    {
        scene.StartScene();
        ShowInfo();
        var commandList = new List<Text>();
        var commands = new List<PauseMenuOption>();
        foreach (var (_, handler, name) in GameGlobal.PauseMenuHandlers.GetAllAvailable())
        {
            commandList.Add(name);
            commands.Add(handler);
        }

        var endScene = false;
        while (!cancellationToken.IsCancellationRequested)
        {
            var choice = await scene.ShowCommands(commandList, cancellationToken);
            if (!choice.HasValue)
            {
                GameGlobal.AudioPlayService.PlayCloseMenuSE();
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
