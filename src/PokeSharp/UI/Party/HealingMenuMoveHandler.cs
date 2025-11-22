using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Items;
using PokeSharp.PokemonModel;

namespace PokeSharp.UI.Party;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HealingMenuMoveHandler : IPartyMenuMoveHandler
{
    private static readonly Name MilkDrink = "MILKDRINK";
    private static readonly Name SoftBoiled = "SOFTBOILED";

    private static readonly Text NotEnoughHP = Text.Localized(
        "HealingMenuMoveHandler",
        "NotEnoughHP",
        "Not enough HP..."
    );
    private static readonly Text UseOnWhichPokemon = Text.Localized(
        "HealingMenuMoveHandler",
        "UseOnWhichPokemon",
        "Use on which Pokemon?"
    );
    private static readonly Text CantUseOnSelf = Text.Localized(
        "HealingMenuMoveHandler",
        "CantUseOnSelf",
        "{0} can't use {1} on itself!"
    );
    private static readonly Text CantUseOnEgg = Text.Localized(
        "HealingMenuMoveHandler",
        "CantUseOnEgg",
        "{0} can't used on an Egg!"
    );
    private static readonly Text CantUseOnPokemon = Text.Localized(
        "HealingMenuMoveHandler",
        "CantUseOnPokemon",
        "{0} can't used on that Pokemon!"
    );
    private static readonly Text HPRestored = Text.Localized(
        "HealingMenuMoveHandler",
        "HPRestored",
        "{0}'s HP was restored by {1} points."
    );

    public IEnumerable<Name> MoveIds => [MilkDrink, SoftBoiled];

    public async ValueTask Handle(
        Pokemon pokemon,
        PokemonMove move,
        int partyIndex,
        PokemonPartyScreen screen,
        CancellationToken cancellationToken = default
    )
    {
        var amount = Math.Max(pokemon.MaxHP / 5, 1);
        if (pokemon.HP <= amount)
        {
            await screen.Display(NotEnoughHP, cancellationToken);
        }
        screen.Scene.HelpText = UseOnWhichPokemon;
        while (!cancellationToken.IsCancellationRequested)
        {
            screen.Scene.PreSelect(partyIndex);
            var newPartyIndex = await screen.ChoosePokemon(cancellationToken: cancellationToken);
            if (newPartyIndex is null)
                break;

            var newPokemon = screen.Party[newPartyIndex.Value];
            var moveName = move.Name;
            if (newPartyIndex == partyIndex)
            {
                await screen.Display(Text.Format(CantUseOnSelf, pokemon.Name, moveName), cancellationToken);
            }
            else if (newPokemon.IsEgg)
            {
                await screen.Display(Text.Format(CantUseOnEgg, moveName), cancellationToken);
            }
            else if (newPokemon.IsFainted || newPokemon.HP == newPokemon.MaxHP)
            {
                await screen.Display(Text.Format(CantUseOnPokemon, moveName), cancellationToken);
            }
            else
            {
                pokemon.HP -= amount;
                var hpGain = newPokemon.ItemRestoreHP(amount);
                await screen.Display(Text.Format(HPRestored, newPokemon.Name, hpGain), cancellationToken);
                screen.Refresh(newPartyIndex.Value);
            }

            if (pokemon.HP <= amount)
                break;
        }
    }
}
