using Microsoft.CodeAnalysis;
using RhoMicro.CodeAnalysis.Generated;

namespace PokeSharp.Compiler.SourceGenerator;

[Generator]
public class CopyFileSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncludedFileSources.RegisterToContext(context);
    }
}
