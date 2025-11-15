using PokeSharp.Game;
using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Happiness;

public interface IHappinessChangeAdjuster
{
    int Priority { get; }

    int AdjustHappinessChange(Pokemon pokemon, HappinessChangeMethod method, int change);
}
