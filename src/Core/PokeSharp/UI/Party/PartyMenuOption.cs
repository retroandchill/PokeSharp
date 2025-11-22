using PokeSharp.PokemonModel;

namespace PokeSharp.UI.Party;

public delegate ValueTask<bool> PartyMenuCommandEffect(
    PokemonPartyScreen screen,
    IReadOnlyList<Pokemon> party,
    int index,
    CancellationToken cancellationToken = default
);

public readonly record struct PartyMenuOptionArgs(PokemonPartyScreen Screen, IReadOnlyList<Pokemon> Party, int Index);

public sealed record PartyMenuOption : IMenuOption<PartyMenuOptionArgs>
{
    public required HandlerName Name { get; init; }

    public required int? Order { get; init; }

    public Func<PartyMenuOptionArgs, bool>? Condition { get; init; }

    public required PartyMenuCommandEffect Effect { get; init; }
}
