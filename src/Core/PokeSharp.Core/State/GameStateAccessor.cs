using Microsoft.Extensions.DependencyInjection;

namespace PokeSharp.Core.State;

public interface IGameStateAccessor<T>
{
    T Current { get; }

    event Action<T> OnChange;

    void Replace(T value);
}

public sealed class GameStateAccessor<T>(T initial) : IGameStateAccessor<T>
{
    public T Current { get; private set; } = initial;

    public event Action<T>? OnChange;

    public void Replace(T value)
    {
        Current = value;
        OnChange?.Invoke(value);
    }
}

public static class GameStateAccessorExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddGameState<T>()
            where T : new()
        {
            return services.AddGameState(_ => new T());
        }

        public IServiceCollection AddGameState<T>(Func<IServiceProvider, T> factory)
        {
            return services
                .AddSingleton<GameStateAccessor<T>>(sp => new GameStateAccessor<T>(factory(sp)))
                .AddSingleton<IGameStateAccessor<T>>(sp => sp.GetRequiredService<GameStateAccessor<T>>());
        }

        public IServiceCollection AddGameState<T>(T instance)
        {
            return services
                .AddSingleton(new GameStateAccessor<T>(instance))
                .AddSingleton<IGameStateAccessor<T>>(sp => sp.GetRequiredService<GameStateAccessor<T>>());
        }
    }
}
