using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Injectio.Attributes;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Core.Data;
using PokeSharp.Data.Pbs;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton]
public partial class RegionalDexCompiler(IDataLoader dataLoader) : IPbsCompiler
{
    public int Order => 14;

    private readonly string _path = Path.Join("PBS", "regional_dexes.txt");

    [CreateSyncVersion]
    public async Task CompileAsync(PbsSerializer serializer, CancellationToken cancellationToken = default)
    {
        var dexLists = new OrderedDictionary<int, List<SpeciesForm>>();

        int? section = null;
        var fileLineData = new FileLineData(_path);
        await foreach (var (line, lineNumber) in serializer.ParsePreppedLinesAsync(_path, cancellationToken))
        {
            fileLineData = fileLineData.WithLine(line, lineNumber);
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

        await dataLoader.SaveEntitiesAsync(
            dexLists.Select(x => new RegionalDex(x.Key, [.. x.Value.Select(y => y.Species)])),
            "regional_dexes",
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
    public Task WriteToFileAsync(PbsSerializer serializer, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    [GeneratedRegex(@"^\s*\[\s*(\d+)\s*\]\s*$")]
    private static partial Regex SectionHeaderRegex { get; }
}
