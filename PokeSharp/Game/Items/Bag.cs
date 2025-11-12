using PokeSharp.Abstractions;

namespace PokeSharp.Game.Items;

public class Bag
{
    public static Bag Instance { get; } = new();

    public bool HasItem(Name item) => false;

    public void RemoveItem(Name item) { }
}
