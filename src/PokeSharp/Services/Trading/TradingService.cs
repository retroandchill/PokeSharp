using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Trading;

[RegisterSingleton]
[AutoServiceShortcut]
public class TradingService(IEnumerable<ICanTradeEvaluator> canTradeEvaluators)
{
    private readonly ImmutableArray<ICanTradeEvaluator> _canTradeEvaluators =
    [
        .. canTradeEvaluators.OrderBy(x => x.Priority),
    ];

    public bool CanTrade(Pokemon pokemon)
    {
        return _canTradeEvaluators.All(x => x.CanTrade(pokemon));
    }
}
