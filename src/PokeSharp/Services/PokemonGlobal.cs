using PokeSharp.Core;

namespace PokeSharp.Services;

[AutoServiceShortcut]
public class PokemonGlobal
{
    public bool IsCycling { get; }

    public bool IsSurfing { get; }

    public bool IsDiving { get; }
}
