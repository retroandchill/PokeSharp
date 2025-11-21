using PokeSharp.Core;
using PokeSharp.Items;
using PokeSharp.PokemonModel;
using PokeSharp.Services;

namespace PokeSharp.UI;

public enum CanSwitch : byte
{
    CannotSwitch,
    CanSwitch,
    Switching,
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

    ValueTask<int?> DisplayCommands(
        Text helpText,
        IReadOnlyList<Text> commands,
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

    ValueTask<int?> ChoosePokemon(
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
        scene.StartScene(party, helpText, annotations);
    }

    public async ValueTask<int?> ChoosePokemon(Text? helpText = null, CancellationToken cancellationToken = default)
    {
        if (helpText is not null)
            scene.HelpText = helpText.Value;

        return await scene.ChoosePokemon(cancellationToken: cancellationToken);
    }

    public async ValueTask<bool> PokemonGiveScreen(Name item, CancellationToken cancellationToken = default)
    {
        scene.StartScene(party, GiveItemPrompt);
        var pokemonId = await scene.ChoosePokemon(cancellationToken: cancellationToken);
        var result = false;
        if (pokemonId is not null)
        {
            result = await GiveItemToPokemon(item, party[pokemonId.Value], this, pokemonId, cancellationToken);
            Refresh(pokemonId.Value);
        }
        scene.EndScene();
        return result;
    }

    public async ValueTask PokemonGiveMailScreen(int mailIndex, CancellationToken cancellationToken = default)
    {
        scene.StartScene(party, GiveItemPrompt);
        var pokemonId = await scene.ChoosePokemon(cancellationToken: cancellationToken);
        if (pokemonId is not null)
        {
            var pokemon = party[pokemonId.Value];
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
        scene.EndScene();
    }

    public void EndScene() => scene.EndScene();

    public void HardRefresh() => scene.HardRefresh();

    public void Refresh() => scene.Refresh();

    public void Refresh(int index) => scene.Refresh(index);

    public async ValueTask Display(Text text, CancellationToken cancellationToken = default)
    {
        await scene.Display(text, cancellationToken);
    }

    public async ValueTask<bool> DisplayConfirm(Text text, CancellationToken cancellationToken = default)
    {
        return await scene.DisplayConfirm(text, cancellationToken);
    }

    public async ValueTask<int?> DisplayCommands(
        Text helpText,
        IReadOnlyList<Text> commands,
        int index = 0,
        CancellationToken cancellationToken = default
    )
    {
        return await scene.DisplayCommands(helpText, commands, index, cancellationToken);
    }

    public async ValueTask Switch(int oldIndex, int newIndex, CancellationToken cancellationToken = default)
    {
        if (oldIndex == newIndex)
            return;

        await scene.BeginSwitch(oldIndex, newIndex, cancellationToken);
        (party[oldIndex], party[newIndex]) = (party[newIndex], party[oldIndex]);
        await scene.EndSwitch(newIndex, cancellationToken);
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
        if (!scene.HasAnnotations)
            return;

        var annotations = party.Select(pokemon => ablePredicate(pokemon) ? Able : NotAble).ToList();
        scene.Annotate(annotations);
    }

    public void ClearAnnotations() => scene.Annotate(null);
}
