using PokeSharp.Abstractions;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Game;

public record PokemonOwner(int Id, Text Name, TrainerGender Gender, Name Language)
{
    public static PokemonOwner FromNewTrainer(Trainer trainer)
    {
        return new PokemonOwner(trainer.Id, trainer.Name, trainer.Gender, trainer.Language);
    }

    public int PublicId => Id & 0xFFFF;
}
