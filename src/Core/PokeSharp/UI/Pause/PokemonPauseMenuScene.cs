using PokeSharp.Core;
using PokeSharp.Core.Strings;

namespace PokeSharp.UI.Pause;

public interface IPokemonPauseMenuScene : IScene
{
    ValueTask StartScene(CancellationToken cancellationToken = default);

    void ShowInfo(Text text);

    void ShowMenu();

    void HideMenu();

    ValueTask<int?> ShowCommands(IReadOnlyList<Text> handlers, CancellationToken cancellationToken = default);

    void Refresh();
}
