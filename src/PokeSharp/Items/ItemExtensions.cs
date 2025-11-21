using PokeSharp.Core;
using PokeSharp.PokemonModel;
using PokeSharp.UI;

namespace PokeSharp.Items;

public static class ItemExtensions
{
    #region Restore HP

    public static int ItemRestoreHP(this Pokemon pokemon, int retoreHP)
    {
        var newHP = pokemon.HP + retoreHP;
        if (newHP > pokemon.MaxHP)
        {
            newHP = pokemon.MaxHP;
        }

        var hpGain = newHP - pokemon.HP;
        pokemon.HP = newHP;
        return hpGain;
    }

    #endregion

    public static ValueTask<bool> GiveItemToPokemon(
        this Pokemon pokemon,
        Name item,
        IScreen screen,
        int pokemonId = 0,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
