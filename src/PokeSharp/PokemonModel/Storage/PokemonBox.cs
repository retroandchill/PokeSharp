using System.Collections;
using PokeSharp.Abstractions;

namespace PokeSharp.PokemonModel.Storage;

public class PokemonBox : IReadOnlyList<Pokemon?>
{
    public Pokemon?[] Pokemon { get; }
    public Text Name { get; set; }
    public int Background { get; set; }

    public const int BoxWidth = 6;
    public const int BoxHeight = 5;
    public const int BoxSize = BoxWidth * BoxHeight;

    public PokemonBox(Text name, int maxPokemon = BoxSize)
    {
        Name = name;
        Background = 0;
        Pokemon = new Pokemon?[maxPokemon];
    }

    public int Count => Pokemon.Length;

    public int PokemonCount => Pokemon.Count(p => p is not null);

    public bool IsFull => PokemonCount == Count;

    public bool IsEmpty => PokemonCount == 0;

    public Pokemon? this[int index]
    {
        get => Pokemon[index];
        set => Pokemon[index] = value;
    }

    public IEnumerator<Pokemon?> GetEnumerator() => ((IEnumerable<Pokemon?>)Pokemon).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Pokemon.GetEnumerator();

    public Span<Pokemon?> AsSpan() => Pokemon;

    public ReadOnlySpan<Pokemon?> AsReadOnlySpan() => Pokemon;

    public static implicit operator Span<Pokemon?>(PokemonBox box) => box.AsSpan();

    public static implicit operator ReadOnlySpan<Pokemon?>(PokemonBox box) => box.AsReadOnlySpan();

    public void Clear()
    {
        for (var i = 0; i < Pokemon.Length; i++)
        {
            Pokemon[i] = null;
        }
    }
}
