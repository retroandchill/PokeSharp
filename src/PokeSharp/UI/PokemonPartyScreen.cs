using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Injectio.Attributes;
using PokeSharp.BattleSystem.Challenge;
using PokeSharp.Core;
using PokeSharp.Items;
using PokeSharp.Overworld;
using PokeSharp.PokemonModel;
using PokeSharp.PokemonModel.Components;
using PokeSharp.Services;
using PokeSharp.Services.Trading;
using PokeSharp.Settings;
using PokeSharp.State;
using PokeSharp.Trainers;

namespace PokeSharp.UI;

public enum CanSwitch : byte
{
    CannotSwitch,
    CanSwitch,
    Switching,
}

public enum EntryEligibility : byte
{
    Ineligible = 0,
    NotEntered = 1,
    Banned = 2,
    Max = 3,
}

public enum PokemonSelectionMode : sbyte
{
    Canceled = -1,
    Selection = 0,
    Switching = 1,
}

public readonly struct PartyScreenSelection(
    int index,
    PokemonSelectionMode selectionMode = PokemonSelectionMode.Selection
)
{
    public int? Index => Selection != PokemonSelectionMode.Canceled ? index : null;

    public PokemonSelectionMode Selection { get; } = selectionMode;

    public PartyScreenSelection()
        : this(-1, PokemonSelectionMode.Canceled) { }

    public static implicit operator int?(PartyScreenSelection selection) => selection.Index;

    public static implicit operator PartyScreenSelection(int index) => new(index);
}

public readonly record struct PartyMenuOptionArgs(PokemonPartyScreen Screen, IReadOnlyList<Pokemon> Party, int Index);

public readonly record struct PartyMenuCommand(Text Name, int? ColorKey = null)
{
    public static implicit operator PartyMenuCommand(Text text) => new(text);
}

public sealed record PartyMenuOption : IMenuOption<PartyMenuOptionArgs>
{
    public required HandlerName Name { get; init; }

    public required int? Order { get; init; }

    public Func<PartyMenuOptionArgs, bool>? Condition { get; init; }

    public required Func<
        PokemonPartyScreen,
        IReadOnlyList<Pokemon>,
        int,
        CancellationToken,
        ValueTask<bool>
    > Effect { get; init; }
}

public readonly struct PartyMenuCommandData
{
    private readonly PartyMenuOption? _option;
    private readonly int _moveIndex;

    public PartyMenuCommandData(PartyMenuOption option)
    {
        _option = option;
        _moveIndex = -1;
    }

    public PartyMenuCommandData(int moveIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(moveIndex);
        _option = null;
        _moveIndex = moveIndex;
    }

    public static implicit operator PartyMenuCommandData(PartyMenuOption option) => new(option);

    public static implicit operator PartyMenuCommandData(int moveIndex) => new(moveIndex);

    public void Match(Action<PartyMenuOption> onOption, Action<int> onMove)
    {
        if (_option is not null)
            onOption(_option);
        else
            onMove(_moveIndex);
    }

    public T Match<T>(Func<PartyMenuOption, T> onOption, Func<int, T> onMove) =>
        _option is not null ? onOption(_option) : onMove(_moveIndex);
}

public interface IPartyMenuMoveHandler
{
    IEnumerable<Name> MoveIds { get; }

    ValueTask Handle(
        Pokemon pokemon,
        PokemonMove move,
        int partyIndex,
        PokemonPartyScreen screen,
        CancellationToken cancellationToken = default
    );
}

[RegisterSingleton]
[AutoServiceShortcut]
public sealed class PartyMenuMoveHandlers(IEnumerable<IPartyMenuMoveHandler> handlers)
{
    private readonly Dictionary<Name, IPartyMenuMoveHandler> _handlers = handlers
        .SelectMany(x => x.MoveIds.Select(moveId => (MoveId: moveId, Handler: x)))
        .ToDictionary(x => x.MoveId, x => x.Handler);

    public bool TryGetHandler(Name moveId, [NotNullWhen(true)] out IPartyMenuMoveHandler? handler) =>
        _handlers.TryGetValue(moveId, out handler);
}

public interface IPokemonPartyScene
{
    void StartScene(
        List<Pokemon> party,
        Text startHelpText,
        IReadOnlyList<Text>? annotations = null,
        bool multiSelect = false,
        bool canAccessStorage = false
    );

    void EndScene();

    ValueTask Display(Text text, CancellationToken cancellationToken = default);

    ValueTask<bool> DisplayConfirm(Text text, CancellationToken cancellationToken = default);

    ValueTask<int?> ShowCommands(
        Text helpText,
        IReadOnlyList<PartyMenuCommand> commands,
        int index = 0,
        CancellationToken cancellationToken = default
    );

    ValueTask<int> ChooseNumber(Text helpText, int max, int initial = 1, CancellationToken cancellationToken = default);

    Text HelpText { set; }

    bool HasAnnotations { get; }

    void Annotate(IReadOnlyList<Text>? annotations);

    void Select(int itemIndex);

    void PreSelect(int itemIndex);

    ValueTask BeginSwitch(int oldIndex, int newIndex, CancellationToken cancellationToken = default);

    ValueTask EndSwitch(int newIndex, CancellationToken cancellationToken = default);

    void ClearSwitching();

    ValueTask ShowSummary(
        int pokemonIndex,
        bool inBattle = false,
        Func<ValueTask>? onScene = null,
        CancellationToken cancellationToken = default
    );

    ValueTask<Name?> ChooseItem(PokemonBag bag, CancellationToken cancellationToken = default);

    ValueTask<Name?> UseItem(
        PokemonBag bag,
        Name item,
        Func<ValueTask>? afterUseItem = null,
        CancellationToken cancellationToken = default
    );

    ValueTask<PartyScreenSelection> ChoosePokemon(
        bool switching = false,
        int? initialSelection = null,
        CanSwitch canSwitch = CanSwitch.CannotSwitch,
        CancellationToken cancellationToken = default
    );

    void HardRefresh();

    void Refresh();

    void Refresh(int index);
}

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
        Scene.StartScene(Party, GiveItemPrompt);
        int? pokemonId = await Scene.ChoosePokemon(cancellationToken: cancellationToken);
        var result = false;
        if (pokemonId is not null)
        {
            result = await Party[pokemonId.Value].GiveItemToPokemon(item, this, pokemonId.Value, cancellationToken);
            Refresh(pokemonId.Value);
        }
        Scene.EndScene();
        return result;
    }

    public async ValueTask PokemonGiveMailScreen(int mailIndex, CancellationToken cancellationToken = default)
    {
        Scene.StartScene(Party, GiveItemPrompt);
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
        Scene.EndScene();
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
        Scene.StartScene(Party, ChooseAndConfirm, annotations, true);
        while (true)
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

        Scene.EndScene();
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
        Scene.StartScene(Party, Party.Count > 1 ? Choose : ChooseOrCancel, annotations);
        while (true)
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

        Scene.EndScene();
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
        while (true)
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
        while (true)
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

public static class PokemonPartyMenuExtensions
{
    private static CachedService<MenuHandlers<PartyMenuOption, PartyMenuOptionArgs>> _cachedService;

    extension(GameGlobal)
    {
        public static MenuHandlers<PartyMenuOption, PartyMenuOptionArgs> PartyMenuHandlers => _cachedService.Instance;
    }
}
