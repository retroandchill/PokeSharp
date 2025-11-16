using Injectio.Attributes;
using PokeSharp.Game;
using PokeSharp.PokemonModel;
using PokeSharp.Services.Healing;

namespace PokeSharp.Services.Evolution;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class ReadyToEvolveSignalHandler : IPokemonHealHandler, IPokemonFaintHandler
{
    public int Priority => 10;

    public void OnFaint(Pokemon pokemon)
    {
        pokemon.RemoveTag(PokemonTags.ReadyToEvolve);
    }

    public void OnFullyHealed(Pokemon pokemon)
    {
        pokemon.RemoveTag(PokemonTags.ReadyToEvolve);
    }
}
