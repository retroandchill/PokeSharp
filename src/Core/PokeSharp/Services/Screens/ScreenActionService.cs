using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Screens;

[AutoServiceShortcut]
public interface IScreenActionService
{
    static IScreenActionService Instance => throw new NotImplementedException();

    Task EvolvePokemonScreen(Pokemon pokemon, Name species);
}
