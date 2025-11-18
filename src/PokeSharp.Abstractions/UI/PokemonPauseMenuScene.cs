using PokeSharp.Core;

namespace PokeSharp.UI;

public sealed record PauseMenuOption : IMenuOption<NullContext>
{
    public required HandlerName Name { get; init; }

    public required int? Order { get; init; }

    public Func<NullContext, bool>? Condition { get; init; }

    public required Func<IPokemonPauseMenuScene, CancellationToken, ValueTask<bool>> Effect { get; init; }
}

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
