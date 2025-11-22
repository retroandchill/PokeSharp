using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Trading;

public interface ICanTradeEvaluator
{
    int Priority { get; }

    bool CanTrade(Pokemon pokemon);
}
