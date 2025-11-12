using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Abstractions;
using PokeSharp.Core.Data;
using Retro.ReadOnlyParams.Annotations;

namespace PokeSharp.Core;

/// <summary>
/// Represents the execution context of a game, managing services and resources for the game lifecycle.
/// This class is sealed to prevent inheritance and ensure proper handling of its resources.
/// </summary>
/// <param name="services">The service provider used to resolve services for the game context.</param>
/// <param name="contextId">The unique identifier for the game context.</param>
public sealed class GameContext([ReadOnly] IServiceProvider services, Name contextId) : IDisposable
{
    /// <summary>
    /// Gets the unique identifier for the game context.
    /// This identifier is used to distinguish between different instances of game contexts
    /// within the application, ensuring proper resource and service management.
    /// </summary>
    public Name ContextId { get; } = contextId;
    private bool _disposed;

    /// <summary>
    /// Gets the data loader responsible for handling the loading and saving of game data.
    /// This property is used to interact with the data management layer of the application,
    /// enabling operations such as retrieving and persisting game-related entities.
    /// </summary>
    [field: AllowNull]
    public IDataLoader DataLoader
    {
        get
        {
            field ??= GetService<IDataLoader>();
            return field;
        }
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
        return services.GetRequiredService<T>();
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
        service = services.GetService<T>();
        return service is not null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        if (services is IDisposable disposable)
            disposable.Dispose();

        _disposed = true;
    }
}
