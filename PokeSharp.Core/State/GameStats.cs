namespace PokeSharp.Core.State;

public class GameStats
{
    public static GameStats Instance { get; } = new();

    public int PokerusInfections { get; set; }
}
