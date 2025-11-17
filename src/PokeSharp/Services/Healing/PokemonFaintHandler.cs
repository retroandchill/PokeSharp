using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Healing;

public interface IPokemonFaintHandler
{
    int Priority { get; }

    void OnFaint(Pokemon pokemon);
}
