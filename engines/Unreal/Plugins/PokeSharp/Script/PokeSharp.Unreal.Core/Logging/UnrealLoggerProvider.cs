using Microsoft.Extensions.Logging;

namespace PokeSharp.Unreal.Core.Logging;

public sealed class UnrealLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new UnrealConsoleLogger(categoryName);
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}
