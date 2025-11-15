using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Happiness;

public interface IHappinessChangeBlocker
{
    int Priority { get; }

    bool ShouldBlockHappinessChange(Pokemon pokemon);
}
