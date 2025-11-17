using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core.Data;
using Retro.ReadOnlyParams.Annotations;

namespace PokeSharp.Core;

/// <summary>
/// Marker class used for the creation of static extension methods to retrieve services from the game context, by name
/// without using reflection every time.
/// </summary>
public static class GameServices;

/// <summary>
/// Represents the execution context of a game, managing services and resources for the game lifecycle.
/// This class is sealed to prevent inheritance and ensure proper handling of its resources.
/// </summary>
public sealed class GameContext : IDisposable
{
    private static GameContext? _instance;

    /// <summary>
    /// Gets the currently active <see cref="GameContext"/> instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">If no <see cref="GameContext"/> exists.</exception>
    public static GameContext Instance =>
        _instance ?? throw new InvalidOperationException("No GameContext available. Call Initialize() first.");

    /// <summary>
    /// Gets a value indicating whether a <see cref="GameContext"/> is currently active.
    /// This property evaluates to true if there is either a globally initialized
    /// <see cref="GameContext"/> or at least one scoped context exists in the stack.
    /// </summary>
    public static bool Exists => _instance is not null;

    /// <summary>
    /// The current generation of the game context. The generation is increased every time the global context is reset.
    /// </summary>
    public static int Generation { get; private set; }

    private readonly IServiceProvider _services;
    private bool _disposed;

    internal GameContext(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Initializes the global game context with the specified <see cref="GameContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="GameContext"/> instance to initialize as the global context.</param>
    /// <exception cref="InvalidOperationException">Thrown when a global game context has already been initialized.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the provided context is <c>null</c>.</exception>
    public static void Initialize(GameContext context)
    {
        if (_instance is not null)
            throw new InvalidOperationException("GameContext already initialized.");

        Generation++;
        _instance = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Resets the state of the <see cref="GameContext"/> instance by disposing and clearing the context.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if an operation fails during the disposal of contexts.</exception>
    public static void Reset()
    {
        _instance?.Dispose();
        _instance = null;
    }

    /// <summary>
    /// Resolves and retrieves a service of the specified type from the game context's service provider.
    /// Throws an exception if the requested service is not registered or if the game context has been disposed.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve from the service provider. Must be a reference type.</typeparam>
    /// <returns>The resolved service of type <typeparamref name="T"/> from the service provider.</returns>
    public T GetService<T>()
        where T : class
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _services.GetRequiredService<T>();
    }

    /// <summary>
    /// Attempts to resolve and retrieve a service of the specified type from the game context's service provider.
    /// Returns a value indicating whether the service was successfully retrieved.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve from the service provider. Must be a reference type.</typeparam>
    /// <param name="service">When this method returns, contains the resolved service of type <typeparamref name="T"/> if successful, or <c>null</c> if the service is not registered.</param>
    /// <returns><c>true</c> if the service was successfully retrieved; otherwise, <c>false</c>.</returns>
    public bool TryGetService<T>([NotNullWhen(true)] out T? service)
        where T : class
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        service = _services.GetService<T>();
        return service is not null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        if (_services is IDisposable disposable)
            disposable.Dispose();

        _disposed = true;
    }
}
