using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Compiler.Core.Serialization;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Core;

[RegisterSingleton]
public sealed partial class PbsCompilerService
{
    private readonly ImmutableArray<IPbsCompiler> _compilers;

    public PbsCompilerService(IEnumerable<IPbsCompiler> compilers)
    {
        _compilers = [.. compilers.OrderBy(x => x.Order)];
    }

    [CreateSyncVersion]
    public async Task CompilePbsFilesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var compiler in _compilers)
        {
            await compiler.CompileAsync(cancellationToken);
        }
    }

    [CreateSyncVersion]
    public async Task WritePbsFilesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var compiler in _compilers)
        {
            await compiler.WriteToFileAsync(cancellationToken);
        }
    }
}
