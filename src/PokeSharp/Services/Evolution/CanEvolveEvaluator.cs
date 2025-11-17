using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Evolution;

public interface ICanEvolveEvaluator
{
    public int Priority { get; }

    public bool CanEvolve(Pokemon pokemon);
}
