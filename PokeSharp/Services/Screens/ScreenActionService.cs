using PokeSharp.Abstractions;
using PokeSharp.Game;

namespace PokeSharp.Services.Screens;

public interface IScreenActionService
{
    static IScreenActionService Instance => throw new NotImplementedException();

    Task EvolvePokemonScreen(Pokemon pokemon, Name species);
}
