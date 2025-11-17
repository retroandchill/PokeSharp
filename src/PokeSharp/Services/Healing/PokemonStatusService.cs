using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.PokemonModel;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Services.Healing;

[RegisterSingleton]
[AutoServiceShortcut]
public class PokemonStatusService(
    IEnumerable<IPokemonHealHandler> healHandlers,
    IEnumerable<IPokemonFaintHandler> faintHandlers
)
{
    private readonly ImmutableArray<IPokemonHealHandler> _healHandlers = [.. healHandlers.OrderBy(x => x.Priority)];
    private readonly ImmutableArray<IPokemonFaintHandler> _faintHandlers = [.. faintHandlers.OrderBy(x => x.Priority)];

    public void OnFullyHealed(Pokemon pokemon)
    {
        foreach (var handler in _healHandlers)
        {
            handler.OnFullyHealed(pokemon);
        }
    }

    public void OnFainted(Pokemon pokemon)
    {
        foreach (var handler in _faintHandlers)
        {
            handler.OnFaint(pokemon);
        }
    }
}
