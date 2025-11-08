using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Abstractions;
using PokeSharp.Core.Data;
using Retro.ReadOnlyParams.Annotations;

namespace PokeSharp.Core;

public sealed class GameContext([ReadOnly] IServiceProvider services, Name contextId) : IDisposable
{
    public Name ContextId { get; } = contextId;
    private bool _disposed;

    [field: AllowNull]
    public IDataLoader DataLoader
    {
        get
        {
            field ??= GetService<IDataLoader>();
            return field;
        }
    }

    public T GetService<T>() where T : class
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return services.GetRequiredService<T>();
    }

    public bool TryGetService<T>([NotNullWhen(true)] out T? service) where T : class
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        service = services.GetService<T>();
        return service is not null;
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        if (services is IDisposable disposable) disposable.Dispose();
        
        _disposed = true;
    }
}