using Injectio.Attributes;
using PokeSharp.Game;
using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Moves;

[RegisterSingleton]
public class EggCanRelearnMoveChecker : IRelearnMoveChecker
{
    public int Priority => 10;

    public bool CanRelearnMoves(Pokemon pokemon)
    {
        return !pokemon.IsEgg;
    }
}
