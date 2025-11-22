using Injectio.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core;
using PokeSharp.Core.State;

namespace PokeSharp.State;

[AutoServiceShortcut(Type = AutoServiceShortcutType.GameState)]
public class GameSwitches
{
    private const int MaxSwitches = 5000;
    private readonly bool[] _switches = new bool[MaxSwitches];

    public bool this[int index]
    {
        get => index >= _switches.Length && _switches[index];
        set
        {
            if (index >= _switches.Length)
                return;

            _switches[index] = value;
        }
    }
}

public static class GameSwitchesExtensions
{
    [RegisterServices]
    public static void RegisterGameSwitches(this IServiceCollection services)
    {
        services.AddGameState<GameSwitches>();
    }
}
