using PokeSharp.Abstractions;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Game;

public abstract class Trainer
{
    public int Id { get; }

    public Text Name { get; }

    public TrainerGender Gender { get; }

    public Name Language { get; }

    public bool IsPartyFull { get; }

    public bool HasPokemonOfType(Name pokemonType) => false;

    public bool HasSpecies(Name speciesId) => false;
}
