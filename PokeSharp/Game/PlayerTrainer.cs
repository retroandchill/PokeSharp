namespace PokeSharp.Game;

public class PlayerTrainer : Trainer
{
    // TODO: Implement this
    public static PlayerTrainer Instance { get; } = new();

    public Pokedex Pokedex { get; } = new();
}
