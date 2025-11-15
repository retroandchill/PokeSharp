using Microsoft.Extensions.DependencyInjection;

namespace PokeSharp.Core;

/// <summary>
/// Provides a builder for configuring and creating a <see cref="GameContext"/> instance.
/// </summary>
/// <remarks>
/// This class allows registering services, adding post-initialization actions, and building a fully configured
/// <see cref="GameContext"/>. It is designed to simplify the setup of a <see cref="GameContext"/> with dependency
/// injection and additional initialization steps.
/// </remarks>
public sealed class GameContextBuilder
{
    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> used to configure and register dependencies for the game context.
    /// </summary>
    /// <remarks>
    /// This property provides access to the collection of service descriptors that can be used to register
    /// services and their dependencies. These services are resolved and managed by the dependency injection
    /// container associated with the <see cref="GameContext"/>. Users can add custom services and configurations
    /// to this collection prior to building the context.
    /// </remarks>
    public IServiceCollection Services { get; } = new ServiceCollection();

    /// <summary>
    /// Builds and returns a fully configured <see cref="GameContext"/> instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="GameContext"/> instance that is configured with the services
    /// and context ID defined in the <see cref="GameContextBuilder"/>.
    /// </returns>
    public GameContext Build()
    {
        var serviceProvider = Services.BuildServiceProvider();
        return new GameContext(serviceProvider);
    }
}
