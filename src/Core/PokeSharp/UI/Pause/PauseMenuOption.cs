namespace PokeSharp.UI.Pause;

public sealed record PauseMenuOption : IMenuOption<NullContext>
{
    public required HandlerName Name { get; init; }

    public required int? Order { get; init; }

    public Func<NullContext, bool>? Condition { get; init; }

    public required Func<IPokemonPauseMenuScene, CancellationToken, ValueTask<bool>> Effect { get; init; }
}
