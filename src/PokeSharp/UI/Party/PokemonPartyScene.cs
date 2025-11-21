using PokeSharp.Core;
using PokeSharp.Items;
using PokeSharp.PokemonModel;

namespace PokeSharp.UI.Party;

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

public readonly record struct PartyMenuCommand(Text Name, int? ColorKey = null)
{
    public static implicit operator PartyMenuCommand(Text text) => new(text);
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

public interface IPokemonPartySceneFactory
{
    IPokemonPartyScene Create();
}
