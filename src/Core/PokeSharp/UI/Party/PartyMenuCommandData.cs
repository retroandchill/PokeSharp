namespace PokeSharp.UI.Party;

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
