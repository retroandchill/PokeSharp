using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UnrealSharp.Log;

namespace PokeSharp.Core.Unreal.Logging;

internal static class LoggerExtensions
{
    public static ELogVerbosity ToLogVerbosity(this LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => ELogVerbosity.VeryVerbose,
            LogLevel.Debug => ELogVerbosity.Verbose,
            LogLevel.Information => ELogVerbosity.Log,
            LogLevel.Warning => ELogVerbosity.Warning,
            LogLevel.Error => ELogVerbosity.Error,
            LogLevel.Critical => ELogVerbosity.Fatal,
            LogLevel.None => ELogVerbosity.NoLogging,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null),
        };
    }

    [RegisterServices]
    public static void RegisterUnrealLogging(this IServiceCollection services)
    {
        services.AddLogging(logging => logging.AddProvider(new UnrealLoggerProvider()));
    }
}
