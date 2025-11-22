using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Moves;

public interface IRelearnMoveChecker
{
    int Priority { get; }

    bool CanRelearnMoves(Pokemon pokemon);
}
