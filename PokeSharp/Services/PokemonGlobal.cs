namespace PokeSharp.Services;

public class PokemonGlobal
{
    public static PokemonGlobal Instance { get; } = new();

    public bool IsCycling { get; }

    public bool IsSurfing { get; }

    public bool IsDiving { get; }
}
