namespace PokeSharp.Compiler.Core.Serialization;

public abstract class PbsException(string? message = null, Exception? inner = null) : Exception(message, inner);


public class PbsFormatException(string? message = null, Exception? inner = null) : PbsException(message, inner);


public class PbsParseException(string? message = null, Exception? inner = null) : PbsException(message, inner);