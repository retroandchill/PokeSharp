using Injectio.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.BattleSystem.BugConstest;
using PokeSharp.BattleSystem.SafariZone;
using PokeSharp.Core;
using PokeSharp.Core.State;
using PokeSharp.Items;

namespace PokeSharp.Services;

[AutoServiceShortcut(Type = AutoServiceShortcutType.GameState)]
public class PokemonGlobal
{
    public List<Mail> Mailbox { get; } = [];

    public bool IsCycling { get; }

    public bool IsSurfing { get; }

    public bool IsDiving { get; }

    public SafariState SafariState { get; } = new();

    public BugContestState BugContestState { get; } = new();
}

public static class PokemonGlobalExtensions
{
    [RegisterServices]
    public static void RegisterPokemonGlobal(IServiceCollection services)
    {
        services.AddGameState<PokemonGlobal>();
    }
}
