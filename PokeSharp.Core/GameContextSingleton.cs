using PokeSharp.Abstractions;

namespace PokeSharp.Core;

public class GameContextSingleton<T>
    where T : class
{
    private T? _instance;
    private Name _currentContext;

    public T Instance
    {
        get
        {
            if (GameContextManager.Current.ContextId != _currentContext || _instance is null)
            {
                _instance = GameContextManager.Current.GetService<T>();
            }

            return _instance;
        }
    }
}
