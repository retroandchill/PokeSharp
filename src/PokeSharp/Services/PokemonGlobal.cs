using Injectio.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core;
using PokeSharp.Core.State;

namespace PokeSharp.Services;

[AutoServiceShortcut(Type = AutoServiceShortcutType.GameState)]
public class PokemonGlobal
{
    public bool IsCycling { get; }

    public bool IsSurfing { get; }

    public bool IsDiving { get; }
}

public static class PokemonGlobalExtensions
{
    [RegisterServices]
    public static void RegisterPokemonGlobal(IServiceCollection services)
    {
        services.AddGameState<PokemonGlobal>();
    }
}
