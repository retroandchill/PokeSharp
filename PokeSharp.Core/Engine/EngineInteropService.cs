namespace PokeSharp.Core.Engine;

public interface IEngineInteropService
{
    static IEngineInteropService Instance => throw new NotImplementedException();

    Task FadeOutInWithMusic(Func<ValueTask> action);
}
