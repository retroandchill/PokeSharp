using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Healing;

public interface IPokemonHealHandler
{
    int Priority { get; }

    void OnFullyHealed(Pokemon pokemon);
}
