namespace PokeSharp.Compiler.Core.Schema;

public class PbsSchemaException(string? message = null, Exception? inner = null) : Exception(message, inner);