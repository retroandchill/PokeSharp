using PokeSharp.Core;

namespace PokeSharp.UI.Pause;

public interface IPokemonPauseMenuScene
{
    void StartScene();

    void ShowInfo(Text text);

    void ShowMenu();

    void HideMenu();

    ValueTask<int?> ShowCommands(IReadOnlyList<Text> handlers, CancellationToken cancellationToken = default);

    void EndScene();

    void Refresh();
}
