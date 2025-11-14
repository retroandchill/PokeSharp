using Injectio.Attributes;
using PokeSharp.Abstractions;
using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Evolution;

[RegisterSingleton]
public class IsEggCanEvolveEvaluator : ICanEvolveEvaluator
{
    public int Priority => 0;

    public bool CanEvolve(Pokemon pokemon)
    {
        return !pokemon.IsEgg;
    }
}

[RegisterSingleton]
public class EverstoneCanEvolveEvaluator : ICanEvolveEvaluator
{
    private static readonly Name Everstone = "EVERSTONE";

    public int Priority => 20;

    public bool CanEvolve(Pokemon pokemon)
    {
        return !pokemon.HasSpecificItem(Everstone);
    }
}

[RegisterSingleton]
public class BattleBondCanEvolveEvaluator : ICanEvolveEvaluator
{
    private static readonly Name BattleBond = "BATTLEBOND";

    public int Priority => 30;

    public bool CanEvolve(Pokemon pokemon)
    {
        return !pokemon.HasSpecificAbility(BattleBond);
    }
}
