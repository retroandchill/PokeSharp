using Microsoft.Extensions.Logging;

namespace PokeSharp.Compiler.Core.Logging;

public static partial class CompilerLogs
{
    [LoggerMessage(LogLevel.Information, "Compiling PBS file `{FileName}`...")]
    public static partial void LogCompilingPbsFile(this ILogger logger, string fileName);

    [LoggerMessage(LogLevel.Information, "Writing PBS file `{FileName}`...")]
    public static partial void LogWritingPbsFile(this ILogger logger, string fileName);
}
