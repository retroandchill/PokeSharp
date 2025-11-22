namespace PokeSharp.UI;

public interface IScene
{
    void EndScene();
}

public sealed class SceneScope(IScene scene) : IDisposable
{
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
            return;
        scene.EndScene();
        _disposed = true;
    }
}
