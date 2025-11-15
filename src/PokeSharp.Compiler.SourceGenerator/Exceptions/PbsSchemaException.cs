using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace PokeSharp.Compiler.SourceGenerator.Exceptions;

public class PbsSchemaException(
    string message,
    ImmutableArray<Diagnostic> diagnostics,
    Exception? innerException = null
) : Exception(message, innerException)
{
    public ImmutableArray<Diagnostic> Diagnostics { get; } = diagnostics;
}
