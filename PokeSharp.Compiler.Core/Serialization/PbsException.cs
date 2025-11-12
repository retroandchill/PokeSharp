using System.Reflection;

namespace PokeSharp.Compiler.Core.Serialization;

public abstract class PbsException(string? message = null, Exception? inner = null) : Exception(message, inner);

public class PbsFormatException(string? message = null, Exception? inner = null) : PbsException(message, inner);

public class PbsParseException(string? message = null, Exception? inner = null) : PbsException(message, inner)
{
    public static PbsParseException Create(Exception e, FileLineData lineData)
    {
        var trueException = e is TargetInvocationException { InnerException: { } innerException } ? innerException : e;
        return new PbsParseException($"{trueException.Message}\n{lineData.LineReport}", e);
    }
}
