namespace PokeSharp.Core;

public sealed class GameContextScope : IDisposable
{
    private readonly GameContext _scopedContext;
    private bool _disposed;

    internal GameContextScope(GameContext context)
    {
        _scopedContext = context;
        GameContextManager.PushContext(_scopedContext);
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        var popped = GameContextManager.PopContext();
        if (popped != _scopedContext) throw new InvalidOperationException("Popped context does not match pushed context.");
        
        _scopedContext.Dispose();
        _disposed = true;
    }
}