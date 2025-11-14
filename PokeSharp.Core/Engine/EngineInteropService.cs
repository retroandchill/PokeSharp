using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Core.Engine;

/// <summary>
/// Abstraction used for interoping between the game engine that this library is running on and the rest of the application.
/// </summary>
[AutoServiceShortcut]
public interface IEngineInteropService
{
    /// <summary>
    /// Fades out the screen and music, then fades it back in.
    /// </summary>
    /// <param name="action">The action to perform during the fadeout.</param>
    /// <returns>A task that when awaited will continue after the screen fades back in.</returns>
    Task FadeOutInWithMusic(Func<ValueTask> action);
}
