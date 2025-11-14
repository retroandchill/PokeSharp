namespace PokeSharp.Core;

/// <summary>
/// Contains a cached instance of a service for the current generation.
/// </summary>
/// <typeparam name="T">The registered type of the service.</typeparam>
public struct CachedService<T>
    where T : class
{
    private WeakReference<T>? _instance;
    private int _currentGeneration;

    /// <summary>
    /// Retrieves the cached instance of the service. If no instance exists, one is fetched and cached.
    /// </summary>
    public T Instance
    {
        get
        {
            if (
                GameContext.Generation == _currentGeneration
                && _instance is not null
                && _instance.TryGetTarget(out var instance)
            )
                return instance;

            instance = GameContext.Instance.GetService<T>();
            if (_instance is null)
            {
                _instance = new WeakReference<T>(instance);
            }
            else
            {
                _instance.SetTarget(instance);
            }

            _currentGeneration = GameContext.Generation;

            return instance;
        }
    }
}
