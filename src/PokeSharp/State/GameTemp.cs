using PokeSharp.Core;

namespace PokeSharp.State;

[AutoServiceShortcut]
public class GameTemp
{
    public bool InStorage { get; }

    public bool InBattle { get; }

    public List<int> PartyCriticalHitsDealt { get; } = [];
    public List<int> PartyDirectDamageTaken { get; } = [];
}
