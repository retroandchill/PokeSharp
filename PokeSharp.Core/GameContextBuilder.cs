using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Abstractions;
using Retro.ReadOnlyParams.Annotations;

namespace PokeSharp.Core;

public sealed class GameContextBuilder([ReadOnly] Name contextId)
{
    public IServiceCollection Services { get; } = new ServiceCollection();
    private readonly List<Action<GameContext>> _postInitializeActions = [];

    public GameContextBuilder OnPostInitialize(Action<GameContext> action)
    {
        _postInitializeActions.Add(action);
        return this;
    }

    public GameContext Build()
    {
        var serviceProvider = Services.BuildServiceProvider();
        var context = new GameContext(serviceProvider, contextId);
        foreach (var action in _postInitializeActions)
        {
            action(context);
        }
        return context;
    }
}
