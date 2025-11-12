namespace PokeSharp.Core;

/// <summary>
/// Provides management for the <see cref="GameContext"/> lifecycle, allowing initialization of a global context,
/// accessing the current active context, and creating scoped contexts for localized game-specific functionality.
/// </summary>
public static class GameContextManager
{
    private static GameContext? _globalContext;

    /// <summary>
    /// Gets the current active <see cref="GameContext"/> from the context stack or the globally
    /// initialized context. If no context is available, an exception is thrown.
    /// </summary>
    public static GameContext Current =>
        _globalContext
        ?? throw new InvalidOperationException("No GameContext available. Call Initialize() or use CreateScope().");

    /// <summary>
    /// Gets a value indicating whether a <see cref="GameContext"/> is currently active.
    /// This property evaluates to true if there is either a globally initialized
    /// <see cref="GameContext"/> or at least one scoped context exists in the stack.
    /// </summary>
    public static bool HasContext => _globalContext is not null;

    /// <summary>
    /// Initializes the global game context with the specified <see cref="GameContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="GameContext"/> instance to initialize as the global context.</param>
    /// <exception cref="InvalidOperationException">Thrown when a global game context has already been initialized.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the provided context is <c>null</c>.</exception>
    public static void Initialize(GameContext context)
    {
        if (_globalContext is not null)
            throw new InvalidOperationException("GameContext already initialized.");

        _globalContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Resets the state of the <see cref="GameContextManager"/> by disposing and clearing the global context and any stacked contexts.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if an operation fails during the disposal of contexts.</exception>
    public static void Reset()
    {
        _globalContext?.Dispose();
        _globalContext = null;
    }
}
