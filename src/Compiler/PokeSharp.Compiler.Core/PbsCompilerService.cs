using System.Collections.Immutable;
using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Core.Data;
using Retro.ReadOnlyParams.Annotations;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Core;

[RegisterSingleton]
public sealed partial class PbsCompilerService(
    [ReadOnly] ILogger<PbsCompilerService> logger,
    [ReadOnly] DataService dataService,
    IOptionsMonitor<PbsCompilerSettings> pbsCompilerSettings,
    IEnumerable<IPbsCompiler> compilers
)
{
    private readonly ImmutableArray<IPbsCompiler> _compilers = [.. compilers.OrderBy(x => x.Order)];

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

    [CreateSyncVersion]
    public async Task RunCompileOnStartAsync(CancellationToken cancellationToken = default)
    {
        var dataFiles = dataService.GetAllDataFilenames().ToArray();
        try
        {
            var mustCompile = false;
            var basePath = new DirectoryInfo(pbsCompilerSettings.CurrentValue.PbsFileBasePath);
            if (!basePath.Exists)
            {
                try
                {
                    basePath.Create();
                }
                catch (IOException e)
                {
                    logger.LogWarning(e, "Could not create base path for PBS files");
                }

                await dataService.LoadGameDataAsync(cancellationToken);
                await WritePbsFilesAsync(cancellationToken);
                mustCompile = true;
            }

            var latestDataTime = DateTime.MinValue;
            var latestPbsTime = DateTime.MinValue;

            foreach (var (fileName, isMandatory) in dataFiles)
            {
                var fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {
                    latestDataTime =
                        fileInfo.LastWriteTimeUtc > latestDataTime ? fileInfo.LastWriteTimeUtc : latestDataTime;
                }
                else if (isMandatory)
                {
                    mustCompile = true;
                }
            }

            latestPbsTime = _compilers
                .SelectMany(x => x.FileNames)
                .Select(p => new FileInfo(p))
                .Where(f => f.Exists)
                .Aggregate(
                    latestPbsTime,
                    (current, filename) => filename.LastWriteTimeUtc > current ? filename.LastWriteTimeUtc : current
                );

            mustCompile |= latestPbsTime > latestDataTime;
            if (pbsCompilerSettings.CurrentValue.AlwaysCompile)
            {
                mustCompile = true;
            }

            if (mustCompile)
            {
                foreach (var dataFile in dataFiles.Select(x => new FileInfo(x.FileName)).Where(x => x.Exists))
                {
                    dataFile.Delete();
                }

                logger.LogInformation("PBS files are newer than data files. Recompiling.");
                await CompilePbsFilesAsync(cancellationToken);
            }
            else
            {
                logger.LogInformation("Data files are up to date.");
            }
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Unknown exception when compiling.");
            foreach (var dataFile in dataFiles.Select(x => new FileInfo(x.FileName)).Where(x => x.Exists))
            {
                dataFile.Delete();
            }

            throw new InvalidOperationException("Unknown exception when compiling.", e);
        }
    }
}
