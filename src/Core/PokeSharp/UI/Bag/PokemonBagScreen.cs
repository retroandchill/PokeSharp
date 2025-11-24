using System.Collections.Immutable;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;
using PokeSharp.Items;
using PokeSharp.Trainers;
using PokeSharp.UI.Party;

namespace PokeSharp.UI.Bag;

public class PokemonBagScreen(IPokemonBagScene scene, PokemonBag bag) : IScreen
{
    private static readonly Text Read = Text.Localized("PokemonBagScreen", "Read", "Read");
    private static readonly Text Use = Text.Localized("PokemonBagScreen", "Use", "Use");
    private static readonly Text Give = Text.Localized("PokemonBagScreen", "Give", "Give");
    private static readonly Text Toss = Text.Localized("PokemonBagScreen", "Toss", "Toss");
    private static readonly Text Deselect = Text.Localized("PokemonBagScreen", "Deselect", "Deselect");
    private static readonly Text Register = Text.Localized("PokemonBagScreen", "Deselect", "Register");
    private static readonly Text Cancel = Text.Localized("PokemonBagScreen", "Cancel", "Cancel");

    private static readonly Text IsSelected = Text.Localized("PokemonBagScreen", "IsSelected", "{0} is selected.");
    private static readonly Text NoPokemon = Text.Localized("PokemonBagScreen", "NoPokemon", "There is no Pokémon.");
    private static readonly Text CantHold = Text.Localized("PokemonBagScreen", "CantHold", "The {0} can't be held.");
    private static readonly Text TossHowMany = Text.Localized(
        "PokemonBagScreen",
        "TossHowMany",
        "Toss out how many {0}?"
    );
    private static readonly Text OkayToThrow = Text.Localized(
        "PokemonBagScreen",
        "TossHowMany",
        "Is it OK to throw away {0} {1}?"
    );
    private static readonly Text ThrewAway = Text.Localized("PokemonBagScreen", "ThrewAway", "Threw away {0} {1}.");

    public async ValueTask<Name?> StartScreen(CancellationToken cancellationToken = default)
    {
        using var sceneScope = scene.StartScene(bag);
        Name? item = null;
        while (!cancellationToken.IsCancellationRequested)
        {
            item = await scene.ChooseItem(cancellationToken);
            if (item is null)
                break;

            var itemData = Item.Get(item.Value);
            int? commandRead = null;
            int? commandUse = null;
            int? commandRegister = null;
            int? commandGive = null;
            int? commandToss = null;
            var commands = new List<Text>(5);

            if (itemData.IsMail)
            {
                commandRead = commands.Count;
                commands.Add(Read);
            }

            var itemHandlers = GameGlobal.ItemHandlers;
            var player = GameGlobal.PlayerTrainer;
            if (itemHandlers.HasOutHandler(item.Value) || (itemData.IsMachine && player.Party.Count > 0))
            {
                commandUse = commands.Count;
                commands.Add(itemHandlers.TryGetUseText(item.Value, out var useText) ? useText : Use);
            }

            if (player.Party.Count > 0 && itemData.CanHold)
            {
                commandGive = commands.Count;
                commands.Add(Give);
            }

            if (itemData.IsImportant)
            {
                commandToss = commands.Count;
                commands.Add(Toss);
            }

            if (bag.IsRegistered(item.Value))
            {
                commandRegister = commands.Count;
                commands.Add(Deselect);
            }
            else if (Item.CanRegister(item.Value))
            {
                commandRegister = commands.Count;
                commands.Add(Register);
            }

            commands.Add(Cancel);

            var itemName = itemData.Name;
            var command = await scene.ShowCommands(
                Text.Format(IsSelected, itemName),
                commands,
                cancellationToken: cancellationToken
            );
            if (commandRead is not null && command == commandRead)
            {
                await GameGlobal.MailService.DisplayMail(new Mail(item.Value, Text.None, Text.None), cancellationToken);
            }
            else if (commandUse is not null && command == commandUse)
            {
                var result = await bag.UseItem(item.Value, scene, cancellationToken);
                if (result == UseFromBagResult.CloseTheBagToUse)
                    break;
                scene.Refresh();
            }
            else if (commandGive is not null && command == commandGive)
            {
                if (player.PokemonCount == 0)
                {
                    await scene.Display(NoPokemon, cancellationToken);
                }
                else if (itemData.IsImportant)
                {
                    await scene.Display(Text.Format(CantHold, itemName), cancellationToken);
                }
                else
                {
                    var partyScene = GameGlobal.PokemonPartySceneFactory.Create();
                    var partyScreen = new PokemonPartyScreen(partyScene, player.Party);
                    await partyScreen.PokemonGiveScreen(item.Value, cancellationToken);
                    scene.Refresh();
                }
            }
            else if (commandToss is not null && command == commandToss)
            {
                var quantity = bag.GetQuantity(item.Value);
                if (quantity > 1)
                {
                    var helpText = Text.Format(TossHowMany, itemData.PortionNamePlural);
                    quantity = await scene.ChooseNumber(helpText, quantity, cancellationToken: cancellationToken);
                }

                if (quantity <= 0)
                    continue;
                itemName = quantity > 1 ? itemData.PortionNamePlural : itemData.PortionName;
                if (!await scene.Confirm(Text.Format(OkayToThrow, quantity, itemName), cancellationToken))
                    continue;

                await scene.Display(Text.Format(ThrewAway, quantity, itemName), cancellationToken);
                bag.Remove(item.Value, quantity);
                scene.Refresh();
            }
            else if (commandRegister is not null && command == commandRegister)
            {
                if (bag.IsRegistered(item.Value))
                {
                    bag.Unregister(item.Value);
                }
                else
                {
                    bag.Register(item.Value);
                }
                scene.Refresh();
            }
        }

        return item;
    }

    public async ValueTask<Name?> ChooseItemScreen(
        Func<Name, bool>? filter = null,
        CancellationToken cancellationToken = default
    )
    {
        var oldLastPocket = bag.LastViewedPocket;
        var oldChoices = bag.LastPocketSelections.ToImmutableArray();
        try
        {
            if (filter is not null)
            {
                bag.ResetLastSelections();
            }

            using var scope = scene.StartScene(bag, true, filter);
            return await scene.ChooseItem(cancellationToken);
        }
        finally
        {
            bag.LastViewedPocket = oldLastPocket;
            for (var i = 0; i < oldChoices.Length; i++)
            {
                bag.LastPocketSelections[i] = oldChoices[i];
            }
        }
    }
}
