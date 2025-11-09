namespace PokeSharp.Core;

public static class GameContextManager
{
    private static readonly Stack<GameContext> ContextStack = new();
    private static GameContext? _globalContext;

    public static GameContext Current
    {
        get
        {
            if (ContextStack.Count > 0)
                return ContextStack.Peek();

            return _globalContext
                ?? throw new InvalidOperationException(
                    "No GameContext available. Call Initialize() or use CreateScope()."
                );
        }
    }

    public static bool HasContext => _globalContext is not null || ContextStack.Count > 0;

    public static void Initialize(GameContext context)
    {
        if (_globalContext is not null)
            throw new InvalidOperationException("GameContext already initialized.");

        _globalContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    public static void Reset()
    {
        _globalContext?.Dispose();
        _globalContext = null;

        foreach (var element in ContextStack)
        {
            element.Dispose();
        }
        ContextStack.Clear();
    }

    public static GameContextScope CreateScope(GameContext context)
    {
        return new GameContextScope(context);
    }

    internal static void PushContext(GameContext context)
    {
        ContextStack.Push(context);
    }

    internal static GameContext PopContext()
    {
        return ContextStack.Count > 0
            ? ContextStack.Pop()
            : throw new InvalidOperationException("No scoped context available.");
    }
}
