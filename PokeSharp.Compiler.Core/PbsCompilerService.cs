using System.Collections.Immutable;
using PokeSharp.Compiler.Core.Serialization;

namespace PokeSharp.Compiler.Core;

public sealed class PbsCompilerService
{
    private readonly ImmutableArray<IPbsCompiler> _compilers;
    private readonly PbsSerializer _serializer = new();

    public PbsCompilerService(IEnumerable<IPbsCompiler> compilers)
    {
        _compilers = [..compilers.OrderBy(x => x.Order)];
    }

    public async Task CompilePbsFiles(CancellationToken cancellationToken = default)
    {
        foreach (var compiler in _compilers)
        {
            await compiler.Compile(_serializer, cancellationToken);
        }
    }
}