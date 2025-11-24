using System.Collections.Immutable;
using PokeSharp.BattleSystem.Challenge;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.Items;
using PokeSharp.Overworld;
using PokeSharp.PokemonModel;
using PokeSharp.Services;
using PokeSharp.Services.Trading;
using PokeSharp.Settings;
using PokeSharp.State;
using PokeSharp.Trainers;

namespace PokeSharp.UI.Party;

public class PokemonPartyScreen(IPokemonPartyScene scene, List<Pokemon> party) : IScreen
{
    public List<Pokemon> Party { get; } = party;
    public IPokemonPartyScene Scene { get; } = scene;

    private static readonly Text GiveItemPrompt = Text.Localized(
        "PokemonPartyScreen",
        "GiveItem",
        "Give to which Pokémon?"
    );
    private static readonly Text CantGiveMail = Text.Localized(
        "PokemonPartyScreen",
        "CantGiveMailText",
        "This Pokémon is holding an item. It can't hold mail."
    );
    private static readonly Text EggsCantHoldMail = Text.Localized(
        "PokemonPartyScreen",
        "EggsCantHoldMail",
        "Eggs can't hold mail."
    );
    private static readonly Text TransferredToMailbox = Text.Localized(
        "PokemonPartyScreen",
        "TransferredToMailbox",
        "Mail was transferred from the Mailbox."
    );
    private static readonly Text MoveNoMaxPP = Text.Localized("PokemonPartyScreen", "MoveNoMaxPP", "{0} (PP: ---)");
    private static readonly Text MoveWithPP = Text.Localized("PokemonPartyScreen", "MoveWithPP", "{0} (PP: {1}/{2})");
    private static readonly Text Able = Text.Localized("PokemonPartyScreen", "Able", "ABLE");
    private static readonly Text NotAble = Text.Localized("PokemonPartyScreen", "NotAble", "NOT ABLE");

    public void StartScene(Text helpText, IReadOnlyList<Text>? annotations = null)
    {
        Scene.StartScene(Party, helpText, annotations);
    }

    public async ValueTask<int?> ChoosePokemon(Text? helpText = null, CancellationToken cancellationToken = default)
    {
        if (helpText is not null)
            Scene.HelpText = helpText.Value;

        return await Scene.ChoosePokemon(cancellationToken: cancellationToken);
    }

    public async ValueTask<bool> PokemonGiveScreen(Name item, CancellationToken cancellationToken = default)
    {
        using var scope = Scene.StartScene(Party, GiveItemPrompt);
        int? pokemonId = await Scene.ChoosePokemon(cancellationToken: cancellationToken);
        var result = false;
        if (pokemonId is not null)
        {
            result = await Party[pokemonId.Value].GiveItemToPokemon(item, this, pokemonId.Value, cancellationToken);
            Refresh(pokemonId.Value);
        }
        return result;
    }

    public async ValueTask PokemonGiveMailScreen(int mailIndex, CancellationToken cancellationToken = default)
    {
        using var scope = Scene.StartScene(Party, GiveItemPrompt);
        int? pokemonId = await Scene.ChoosePokemon(cancellationToken: cancellationToken);
        if (pokemonId is not null)
        {
            var pokemon = Party[pokemonId.Value];
            if (pokemon.HasItem || pokemon.Mail is not null)
            {
                await Display(CantGiveMail, cancellationToken);
            }
            else if (pokemon.IsEgg)
            {
                await Display(EggsCantHoldMail, cancellationToken);
            }
            else
            {
                await Display(TransferredToMailbox, cancellationToken);
                pokemon.Mail = GameGlobal.PokemonGlobal.Mailbox[mailIndex];
                pokemon.ItemId = pokemon.Mail.Item;
                GameGlobal.PokemonGlobal.Mailbox.RemoveAt(mailIndex);
                Refresh(pokemonId.Value);
            }
        }
    }

    public void EndScene() => Scene.EndScene();

    public void HardRefresh() => Scene.HardRefresh();

    public void Refresh() => Scene.Refresh();

    public void Refresh(int index) => Scene.Refresh(index);

    public async ValueTask Display(Text text, CancellationToken cancellationToken = default)
    {
        await Scene.Display(text, cancellationToken);
    }

    public async ValueTask<bool> DisplayConfirm(Text text, CancellationToken cancellationToken = default)
    {
        return await Scene.DisplayConfirm(text, cancellationToken);
    }

    public async ValueTask<int?> DisplayCommands(
        Text helpText,
        IEnumerable<Text> commands,
        int index = 0,
        CancellationToken cancellationToken = default
    )
    {
        return await Scene.ShowCommands(
            helpText,
            commands.Select(x => new PartyMenuCommand(x)).ToArray(),
            index,
            cancellationToken
        );
    }

    public async ValueTask Switch(int oldIndex, int newIndex, CancellationToken cancellationToken = default)
    {
        if (oldIndex == newIndex)
            return;

        await Scene.BeginSwitch(oldIndex, newIndex, cancellationToken);
        (Party[oldIndex], Party[newIndex]) = (Party[newIndex], Party[oldIndex]);
        await Scene.EndSwitch(newIndex, cancellationToken);
    }

    public async ValueTask<int?> ChooseMove(
        Pokemon pokemon,
        Text helpText,
        int index = 0,
        CancellationToken cancellationToken = default
    )
    {
        var moveNames = pokemon
            .Moves.Select(move =>
                move.TotalPP <= 0
                    ? Text.Format(MoveNoMaxPP, move.Name)
                    : Text.Format(MoveWithPP, move.Name, move.PP, move.TotalPP)
            )
            .ToList();
        return await DisplayCommands(helpText, moveNames, index, cancellationToken);
    }

    public void RefreshAnnotations(Func<Pokemon, bool> ablePredicate)
    {
        if (!Scene.HasAnnotations)
            return;

        var annotations = Party.Select(pokemon => ablePredicate(pokemon) ? Able : NotAble).ToList();
        Scene.Annotate(annotations);
    }

    public void ClearAnnotations() => Scene.Annotate(null);

    private static readonly ImmutableArray<Text> Ordinals =
    [
        Text.Localized("PokemonPartyScreen", "Ineligible", "INELIGIBLE"),
        Text.Localized("PokemonPartyScreen", "NotEntered", "NOT ENTERED"),
        Text.Localized("PokemonPartyScreen", "Banned", "BANNED"),
    ];

    private static readonly ImmutableArray<Text> Positions =
    [
        Text.Localized("PokemonPartyScreen", "First", "FIRST"),
        Text.Localized("PokemonPartyScreen", "Second", "SECOND"),
        Text.Localized("PokemonPartyScreen", "Third", "THIRD"),
        Text.Localized("PokemonPartyScreen", "Fourth", "FOURTH"),
        Text.Localized("PokemonPartyScreen", "Fifth", "FIFTH"),
        Text.Localized("PokemonPartyScreen", "Sixth", "SIXTH"),
        Text.Localized("PokemonPartyScreen", "Seventh", "SEVENTH"),
        Text.Localized("PokemonPartyScreen", "Eighth", "EIGHTH"),
        Text.Localized("PokemonPartyScreen", "Ninth", "NINTH"),
        Text.Localized("PokemonPartyScreen", "Tenth", "TENTH"),
        Text.Localized("PokemonPartyScreen", "Eleventh", "ELEVENTH"),
        Text.Localized("PokemonPartyScreen", "Twelfth", "TWELFTH"),
    ];

    private static readonly Text ChooseAndConfirm = Text.Localized(
        "PokemonPartyScreen",
        "ChooseAndConfirm",
        "Choose Pokémon and confirm."
    );
    private static readonly Text Choose = Text.Localized("PokemonPartyScreen", "Choose", "Choose a Pokémon.");
    private static readonly Text ChooseOrCancel = Text.Localized(
        "PokemonPartyScreen",
        "ChooseAndConfirm",
        "Choose Pokémon or cancel."
    );

    private static readonly Text Entry = Text.Localized("PokemonPartyScreen", "Entry", "Entry");
    private static readonly Text NoEntry = Text.Localized("PokemonPartyScreen", "NoEntry", "No Entry");
    private static readonly Text Summary = Text.Localized("PokemonPartyScreen", "Summary", "Summary");
    private static readonly Text Cancel = Text.Localized("PokemonPartyScreen", "Cancel", "Cancel");

    private static readonly Text DoWhatWith = Text.Localized("PokemonPartyScreen", "DoWhatWith", "Do what with {0}?");
    private static readonly Text NoMoreCanEnter = Text.Localized(
        "PokemonPartyScreen",
        "NoMoreCanEnter",
        "No more than {0} Pokémon may enter."
    );

    public async ValueTask<List<Pokemon>?> PokemonMultipleEntryScreen(
        PokemonChallengeRuleset ruleset,
        CancellationToken cancellationToken = default
    )
    {
        var annotations = new Text[Party.Count];
        var statuses = new EntryEligibility[Party.Count];
        var maxPartySize = GameGlobal.GameSettings.MaxPartySize;
        var ordinals = new List<Text>(Ordinals.Length + maxPartySize);
        ordinals.AddRange(Ordinals);
        for (var i = 0; i < maxPartySize; i++)
        {
            if (i < Positions.Length)
            {
                ordinals.Add(Positions[i]);
            }
            else
            {
                ordinals.Add($"{i + 1}th");
            }
        }

        if (ruleset.HasValidTeam(Party))
            return null;

        List<Pokemon>? result = null;
        var addedEntry = false;
        for (var i = 0; i < Party.Count; i++)
        {
            statuses[i] = ruleset.IsPokemonValid(Party[i]) ? EntryEligibility.NotEntered : EntryEligibility.Banned;
            annotations[i] = ordinals[(int)statuses[i]];
        }
        using var scope = Scene.StartScene(Party, ChooseAndConfirm, annotations, true);
        while (!cancellationToken.IsCancellationRequested)
        {
            var realOrder = new List<int>(Party.Count);
            for (var i = 0; i < Party.Count; i++)
            {
                for (var j = 0; j < Party.Count; j++)
                {
                    if ((int)statuses[j] != i + (int)EntryEligibility.Max)
                        continue;

                    realOrder.Add(j);
                    break;
                }
            }

            for (var i = 0; i < realOrder.Count; i++)
            {
                statuses[realOrder[i]] = (EntryEligibility)(i + (int)EntryEligibility.Max);
            }

            for (var i = 0; i < Party.Count; i++)
            {
                annotations[i] = ordinals[(int)statuses[i]];
            }

            Scene.Annotate(annotations);

            if (realOrder.Count == ruleset.Number && addedEntry)
            {
                Scene.Select(maxPartySize);
            }

            Scene.HelpText = ChooseAndConfirm;
            int? selectionIndex = await Scene.ChoosePokemon(cancellationToken: cancellationToken);
            addedEntry = false;
            // This means that confirm was chosen
            if (selectionIndex == maxPartySize)
            {
                result = realOrder.Select(i => Party[i]).ToList();

                var errors = new List<Text>();
                if (ruleset.IsValid(result, errors))
                    break;

                await Display(errors[0], cancellationToken);
                result = null;
            }

            // The player canceled selection
            if (selectionIndex is null)
                break;

            int? commandEntry = null;
            int? commandNoEntry = null;
            var commands = new List<PartyMenuCommand>(3);
            switch (statuses[selectionIndex.Value])
            {
                case EntryEligibility.NotEntered:
                    commandEntry = commands.Count;
                    commands.Add(Entry);
                    break;
                case > EntryEligibility.Banned:
                    commandNoEntry = commands.Count;
                    commands.Add(NoEntry);
                    break;
            }

            var pokemon = Party[selectionIndex.Value];
            var commandSummary = commands.Count;
            commands.Add(Summary);
            commands.Add(Cancel);

            var command = await Scene.ShowCommands(
                Text.Format(DoWhatWith, pokemon.Name),
                commands,
                cancellationToken: cancellationToken
            );
            if (commandEntry is not null && command == commandEntry)
            {
                if (realOrder.Count >= ruleset.Number && ruleset.Number > 0)
                {
                    await Display(Text.Format(NoMoreCanEnter, ruleset.Number), cancellationToken);
                }
                else
                {
                    statuses[selectionIndex.Value] = (EntryEligibility)(realOrder.Count + (int)EntryEligibility.Max);
                    addedEntry = true;
                    Refresh(selectionIndex.Value);
                }
            }
            else if (commandNoEntry is not null && command == commandNoEntry)
            {
                statuses[selectionIndex.Value] = EntryEligibility.NotEntered;
                Refresh(selectionIndex.Value);
            }
            else if (command == commandSummary)
            {
                await Scene.ShowSummary(
                    selectionIndex.Value,
                    cancellationToken: cancellationToken,
                    onScene: () =>
                    {
                        Scene.HelpText = Party.Count > 1 ? Choose : ChooseOrCancel;
                        return ValueTask.CompletedTask;
                    }
                );
            }
        }

        return result;
    }

    public async ValueTask<int?> ChooseAblePokemon(
        Func<Pokemon, bool> ablePredicate,
        bool allowIneligible = false,
        CancellationToken cancellationToken = default
    )
    {
        var annotations = new List<Text>(Party.Count);
        var eligibility = new List<bool>(Party.Count);
        foreach (var elibible in Party.Select(ablePredicate))
        {
            eligibility.Add(elibible);
            annotations.Add(elibible ? Able : NotAble);
        }

        int? result = null;
        using var scope = Scene.StartScene(Party, Party.Count > 1 ? Choose : ChooseOrCancel, annotations);
        while (!cancellationToken.IsCancellationRequested)
        {
            Scene.HelpText = Party.Count > 1 ? Choose : ChooseOrCancel;
            int? pokemonIndex = await Scene.ChoosePokemon(cancellationToken: cancellationToken);
            if (pokemonIndex is null)
                break;

            if (eligibility[pokemonIndex.Value] || allowIneligible)
                continue;

            result = pokemonIndex;
            break;
        }

        return result;
    }

    public async ValueTask<int?> ChooseTradablePokemon(
        Func<Pokemon, bool> ablePredicate,
        bool allowIneligible = false,
        CancellationToken cancellationToken = default
    )
    {
        return await ChooseAblePokemon(
            pkmn => GameGlobal.TradingService.CanTrade(pkmn) && ablePredicate(pkmn),
            allowIneligible,
            cancellationToken
        );
    }

    private static readonly Name PokemonBoxLink = "POKEMONBOXLINK";
    private static readonly Name DisableBoxLink = "DisableBoxLink";

    private static readonly Text MoveToWhere = Text.Localized("PokemonPartyScreen", "MoveToWhere", "Move to where?");

    public async ValueTask<(Pokemon Pokemon, Name MoveId)?> PokemonScreen(CancellationToken cancellationToken = default)
    {
        var canAccessStorage =
            GameGlobal.PlayerTrainer.HasBoxLink
            || GameGlobal.PokemonBag.Has(PokemonBoxLink)
                && !GameGlobal.GameSwitches[GameGlobal.GameSettings.DisableBoxLinkSwitch]
                && !GameGlobal.GameMap.HasMetadataTag(DisableBoxLink);

        Scene.StartScene(Party, Party.Count > 1 ? Choose : ChooseOrCancel, null, false, canAccessStorage);
        while (!cancellationToken.IsCancellationRequested)
        {
            Scene.HelpText = Party.Count > 1 ? Choose : ChooseOrCancel;
            var partyIndex = await Scene.ChoosePokemon(false, null, CanSwitch.CanSwitch, cancellationToken);
            if (partyIndex.Index is null)
                break;

            if (partyIndex.Selection == PokemonSelectionMode.Switching)
            {
                Scene.HelpText = MoveToWhere;
                var oldPartyId = partyIndex.Index.Value;
                partyIndex = await Scene.ChoosePokemon(true, null, CanSwitch.CanSwitch, cancellationToken);

                if (partyIndex.Index is not null && partyIndex.Index != oldPartyId)
                {
                    await Switch(oldPartyId, partyIndex.Index.Value, cancellationToken);
                }

                continue;
            }

            var pokemon = Party[partyIndex.Index.Value];
            var commandList = new List<PartyMenuCommand>();
            var commands = new List<PartyMenuCommandData>();
            foreach (
                var (_, option, name) in GameGlobal.PartyMenuHandlers.GetAllAvailable(
                    new PartyMenuOptionArgs(this, Party, partyIndex.Index.Value)
                )
            )
            {
                commandList.Add(name);
                commands.Add(option);
            }

            commandList.Add(Cancel);

            if (!pokemon.IsEgg)
            {
                var insertIndex = 1;
                var hiddenMoveHandlers = GameGlobal.HiddenMoveHandlers;
                foreach (var (i, move) in pokemon.Moves.Index())
                {
                    if (!hiddenMoveHandlers.HasHandler(move.Id))
                        continue;

                    commandList.Insert(insertIndex, new PartyMenuCommand(move.Name, 1));
                    commands.Insert(insertIndex, i);
                    insertIndex++;
                }
            }

            var choice = await Scene.ShowCommands(
                Text.Format(DoWhatWith, pokemon.Name),
                commandList,
                cancellationToken: cancellationToken
            );
            if (choice is null || choice >= commands.Count)
                continue;

            var moveSelection = await commands[choice.Value]
                .Match(
                    async opt =>
                    {
                        await opt.Effect(this, Party, partyIndex.Index.Value, cancellationToken);
                        return null;
                    },
                    async moveIndex =>
                    {
                        var move = pokemon.Moves[moveIndex];
                        if (GameGlobal.PartyMenuMoveHandlers.TryGetHandler(move.Id, out var handler))
                        {
                            await handler.Handle(pokemon, move, partyIndex.Index.Value, this, cancellationToken);
                        }
                        else if (await pokemon.CanUseHiddenMove(move.Id, cancellationToken: cancellationToken))
                        {
                            if (!await pokemon.ConfirmUseHiddenMove(move.Id, cancellationToken))
                                return null;

                            Scene.EndScene();
                            if (
                                !await pokemon.SelectHiddenMoveOptionBeforeUse(
                                    move.Id,
                                    () =>
                                    {
                                        Scene.StartScene(
                                            Party,
                                            Party.Count > 1 ? Choose : ChooseOrCancel,
                                            null,
                                            false,
                                            canAccessStorage
                                        );
                                        return ValueTask.CompletedTask;
                                    },
                                    cancellationToken
                                )
                            )
                            {
                                return ((Pokemon Pokemon, Name MoveId)?)null;
                            }

                            return (Pokemon: pokemon, MoveId: move.Id);
                        }

                        return null;
                    }
                );

            if (moveSelection is not null)
                return moveSelection;
        }

        return null;
    }
}
