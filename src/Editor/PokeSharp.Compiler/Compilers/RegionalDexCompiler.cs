using System.ComponentModel.DataAnnotations;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Logging;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Core.Utils;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;
using PokeSharp.Services.Evolution;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public partial class RegionalDexCompiler : IPbsCompiler
{
    public int Order => 11;

    private string _path;
    public IEnumerable<string> FileNames => [_path];
    private readonly ILogger<RegionalDexCompiler> _logger;
    private readonly PbsSerializer _serializer;
    private readonly IFileSystem _fileSystem;

    public RegionalDexCompiler(
        IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings,
        ILogger<RegionalDexCompiler> logger,
        PbsSerializer serializer,
        IFileSystem fileSystem
    )
    {
        _path = Path.Join(pbsCompileSettings.CurrentValue.PbsFileBasePath, "regional_dexes.txt");
        pbsCompileSettings.OnChange(x => _path = Path.Join(x.PbsFileBasePath, "regional_dexes.txt"));
        _logger = logger;
        _serializer = serializer;
        _fileSystem = fileSystem;
    }

    [CreateSyncVersion]
    public async Task CompileAsync(CancellationToken cancellationToken = default)
    {
        var dexLists = new OrderedDictionary<int, List<SpeciesForm>>();

        int? section = null;
        _logger.LogCompilingPbsFile(Path.GetFileName(_path));
        await foreach (var (line, _, fileLineData) in _serializer.ParsePreppedLinesAsync(_path, cancellationToken))
        {
            var sectionMatch = SectionHeaderRegex.Match(line);
            if (sectionMatch.Success)
            {
                section = int.Parse(sectionMatch.Groups[1].Value);

                if (dexLists.ContainsKey(section.Value))
                {
                    throw new PbsParseException(
                        $"Dex list number {section.Value} is defined at least twice.\n{fileLineData.LineReport}"
                    );
                }

                dexLists.Add(section.Value, []);
            }
            else
            {
                if (!section.HasValue)
                {
                    throw new PbsParseException(
                        $"Expected a section at the beginning of the file.\n{fileLineData.LineReport}"
                    );
                }

                var speciesList = dexLists[section.Value];
                speciesList.AddRange(
                    line.Split(',')
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(species => ParseSpecies(species, fileLineData))
                );
            }
        }

        foreach (var (index, list) in dexLists)
        {
            var uniqueList = list.Distinct().ToList();
            if (list.Count == uniqueList.Count)
                continue;

            foreach (var (i, species) in list.Index())
            {
                if (uniqueList.Count > i && uniqueList[i] == species)
                    continue;

                throw new ValidationException($"Dex list number {index} has species {species} listed twice.");
            }
        }

        await RegionalDex.ImportAsync(
            dexLists.Select(x => new RegionalDex(x.Key, [.. x.Value.Select(y => y.Species)])),
            cancellationToken
        );
    }

    private static SpeciesForm ParseSpecies(string species, FileLineData fileLineData)
    {
        var trimmed = species.AsSpan().Trim();
        if (Species.TryGet(new Name(trimmed), out var entity))
        {
            return entity.Id;
        }

        throw new PbsParseException(
            $"Undefined species constant name: {species}.\nMake sure the species is defined in PBS/pokemon.txt.\n{fileLineData.LineReport}"
        );
    }

    [CreateSyncVersion]
    public async Task WriteToFileAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWritingPbsFile(Path.GetFileName(_path));
        await _fileSystem.WriteFileWithBackupAsync(_path, WriteAction);
        return;

        async ValueTask WriteAction(StreamWriter fileWriter)
        {
            await PbsSerializer.AddPbsHeaderToFileAsync(fileWriter);

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var (id, speciesList) in RegionalDex.Entities)
            {
                await fileWriter.WriteLineAsync("#-------------------------------");
                await fileWriter.WriteAsync($"[{id}]");
                var comma = false;
                Name[]? currentFamily = null;
                foreach (var species in speciesList.Where(species => !species.IsNone))
                {
                    if (currentFamily is not null && currentFamily.Contains(species) && comma)
                    {
                        await fileWriter.WriteAsync(',');
                    }
                    else
                    {
                        currentFamily = GetDexGrouping(Species.Get(species));
                        await fileWriter.WriteLineAsync();
                    }

                    await fileWriter.WriteAsync(species.ToString());
                    comma = true;
                }
                await fileWriter.WriteLineAsync();
            }
        }
    }

    private static Name[] GetDexGrouping(Species species) => species.GetFamilySpecies().ToArray();

    [GeneratedRegex(@"^\s*\[\s*(\d+)\s*\]\s*$")]
    private static partial Regex SectionHeaderRegex { get; }
}
