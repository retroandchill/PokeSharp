using Injectio.Attributes;
using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Moves;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class EggCanRelearnMoveChecker : IRelearnMoveChecker
{
    public int Priority => 10;

    public bool CanRelearnMoves(Pokemon pokemon)
    {
        return !pokemon.IsEgg;
    }
}
