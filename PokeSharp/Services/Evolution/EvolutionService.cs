using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Game;
using PokeSharp.Services.Happiness;

namespace PokeSharp.Services.Evolution;

[RegisterSingleton]
public class EvolutionService(IEnumerable<ICanEvolveEvaluator> canEvolveEvaluators)
{
    private static readonly GameContextSingleton<EvolutionService> InstanceSingleton = new();

    public static EvolutionService Instance => InstanceSingleton.Instance;

    private readonly ImmutableArray<ICanEvolveEvaluator> _canEvolveEvaluators =
    [
        .. canEvolveEvaluators.OrderBy(e => e.Priority),
    ];

    public bool CanEvolve(Pokemon pokemon)
    {
        return _canEvolveEvaluators.All(e => e.CanEvolve(pokemon));
    }
}
