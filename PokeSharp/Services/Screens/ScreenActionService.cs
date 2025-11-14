using PokeSharp.Abstractions;
using PokeSharp.Game;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Services.Screens;

[AutoServiceShortcut]
public interface IScreenActionService
{
    static IScreenActionService Instance => throw new NotImplementedException();

    Task EvolvePokemonScreen(Pokemon pokemon, Name species);
}
