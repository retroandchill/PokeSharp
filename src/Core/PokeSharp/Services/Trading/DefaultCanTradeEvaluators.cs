using Injectio.Attributes;
using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Trading;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class IsNotEggTradeEvaluator : ICanTradeEvaluator
{
    public int Priority => 10;

    public bool CanTrade(Pokemon pokemon)
    {
        return !pokemon.IsEgg;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class IsNotTaggedCanTradeEvaluator : ICanTradeEvaluator
{
    public int Priority => 30;

    public bool CanTrade(Pokemon pokemon)
    {
        return !pokemon.HasTag(PokemonTags.CannotTrade);
    }
}
