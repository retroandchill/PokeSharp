using PokeSharp.Abstractions;
using PokeSharp.Data.Pbs;
using PokeSharp.Trainers;

namespace PokeSharp.PokemonModel;

public record PokemonOwner(uint Id, Text Name, TrainerGender Gender, Name Language)
{
    public static PokemonOwner FromNewTrainer(Trainer trainer)
    {
        return new PokemonOwner(trainer.Id, trainer.Name, trainer.Gender, trainer.Language);
    }

    public uint PublicId => Trainer.GetPublicId(Id);
}
