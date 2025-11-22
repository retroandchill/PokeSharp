namespace PokeSharp.UI.Party;

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
