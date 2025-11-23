using Microsoft.Extensions.Logging;
using UnrealSharp.Log;

namespace PokeSharp.Unreal.Core.Logging;

public class UnrealConsoleLogger(string categoryName) : ILogger
{
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        if (!IsEnabled(logLevel))
            return;

        UnrealLogger.Log(categoryName, formatter(state, exception), logLevel.ToLogVerbosity());
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return null;
    }
}
