using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Moves;

[RegisterSingleton]
[AutoServiceShortcut]
public class MoveService(IEnumerable<IRelearnMoveChecker> relearnMoveCheckers)
{
    private readonly ImmutableArray<IRelearnMoveChecker> _relearnMoveCheckers =
    [
        .. relearnMoveCheckers.OrderBy(x => x.Priority),
    ];

    public bool CanRelearnMoves(Pokemon pokemon)
    {
        if (!_relearnMoveCheckers.All(x => x.CanRelearnMoves(pokemon)))
            return false;

        var thisLevel = pokemon.Level;
        return pokemon
            .MoveList.Where(m => m.Level <= thisLevel)
            .Select(m => m.Move)
            .Concat(pokemon.FirstMoves)
            .Any(m => !pokemon.HasMove(m));
    }
}
