using PokeSharp.Core;

namespace PokeSharp.UI.Pause;

public interface IPokemonPauseMenuScene : IScene
{
    void StartScene();

    void ShowInfo(Text text);

    void ShowMenu();

    void HideMenu();

    ValueTask<int?> ShowCommands(IReadOnlyList<Text> handlers, CancellationToken cancellationToken = default);

    void Refresh();
}
