namespace PokeSharp.Core.State;

public class GameTemp
{
    // TODO: Implement this
    public static GameTemp Instance { get; } = new();

    public bool InStorage { get; }

    public bool InBattle { get; }

    public List<int> PartyCriticalHitsDealt { get; } = [];
    public List<int> PartyDirectDamageTaken { get; } = [];
}
