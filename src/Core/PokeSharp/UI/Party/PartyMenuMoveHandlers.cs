using System.Diagnostics.CodeAnalysis;
using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.PokemonModel;

namespace PokeSharp.UI.Party;

public interface IPartyMenuMoveHandler
{
    IEnumerable<Name> MoveIds { get; }

    ValueTask Handle(
        Pokemon pokemon,
        PokemonMove move,
        int partyIndex,
        PokemonPartyScreen screen,
        CancellationToken cancellationToken = default
    );
}

[RegisterSingleton]
[AutoServiceShortcut]
public sealed class PartyMenuMoveHandlers(IEnumerable<IPartyMenuMoveHandler> handlers)
{
    private readonly Dictionary<Name, IPartyMenuMoveHandler> _handlers = handlers
        .SelectMany(x => x.MoveIds.Select(moveId => (MoveId: moveId, Handler: x)))
        .ToDictionary(x => x.MoveId, x => x.Handler);

    public bool TryGetHandler(Name moveId, [NotNullWhen(true)] out IPartyMenuMoveHandler? handler) =>
        _handlers.TryGetValue(moveId, out handler);
}
