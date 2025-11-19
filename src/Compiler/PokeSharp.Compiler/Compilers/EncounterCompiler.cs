using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Logging;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Core.Utils;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Core;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using PokeSharp.Maps;
using Retro.ReadOnlyParams.Annotations;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public partial class EncounterCompiler : IPbsCompiler
{
    public int Order => 13;
    public IEnumerable<string> FileNames => [_path];
    private string _path;
    private readonly IMapMetadataRepository _mapMetadataRepository;
    private readonly ILogger<EncounterCompiler> _logger;

    public EncounterCompiler(
        IMapMetadataRepository mapMetadataRepository,
        IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings,
        ILogger<EncounterCompiler> logger
    )
    {
        _path = Path.Join(pbsCompileSettings.CurrentValue.PbsFileBasePath, "encounters.txt");
        pbsCompileSettings.OnChange(x => _path = Path.Join(x.PbsFileBasePath, "encounters.txt"));
        _mapMetadataRepository = mapMetadataRepository;
        _logger = logger;
    }

    [CreateSyncVersion]
    public async Task CompileAsync(CancellationToken cancellationToken = default)
    {
        var maxLevel = GrowthRate.MaxLevel;

        var usedKeys = new HashSet<EncounterId>();
        var encounters = new List<Encounter>();
        EncounterInfo? currentEncounter = null;
        Name? currentType = null;
        _logger.LogCompilingPbsFile(Path.GetFileName(_path));
        await foreach (var (line, _, fileLineData) in PbsSerializer.ParsePreppedLinesAsync(_path, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                if (currentType is not null && SpeciesLine.IsMatch(line))
                {
                    ArgumentNullException.ThrowIfNull(currentEncounter);
                    var values = ParseEntryLine(line, currentEncounter, currentType.Value);

                    if (values.MinLevel > maxLevel)
                    {
                        throw new SerializationException(
                            $"Level number {values.MinLevel} is not valid (max. {maxLevel})."
                        );
                    }

                    if (values.MaxLevel > maxLevel)
                    {
                        throw new SerializationException(
                            $"Level number {values.MaxLevel} is not valid (max. {maxLevel})."
                        );
                    }

                    if (values.MinLevel > values.MaxLevel)
                    {
                        throw new SerializationException("Minimum level is greater than maximum level.");
                    }

                    currentEncounter.Types[currentType.Value].Add(values);
                    continue;
                }

                var headerMatch = MapIdLine.Match(line);
                if (headerMatch.Success)
                {
                    var values = headerMatch.Groups[1].Value.Split(',').Select(int.Parse).ToImmutableArray();
                    var mapNumber = values[0];
                    var mapVersion = values.Length > 1 ? values[1] : 0;

                    if (currentEncounter is not null)
                    {
                        FinalizeEncounter(currentEncounter, encounters);
                    }

                    var key = new EncounterId(mapNumber, mapVersion);
                    if (!usedKeys.Add(key))
                    {
                        throw new SerializationException($"Encounters for map '{mapNumber}' are defined twice.");
                    }

                    currentEncounter = new EncounterInfo { Id = key };
                }
                else if (currentEncounter is null)
                {
                    throw new SerializationException($"Expected a map number, got \"{line}\" instead.");
                }
                else
                {
                    var values = line.Split(',').Select(s => s.Trim()).ToImmutableArray();
                    currentType = !values.IsDefaultOrEmpty ? values[0] : (Name?)null;
                    if (currentType.HasValue && EncounterType.TryGet(currentType.Value, out var type))
                    {
                        currentEncounter.StepChances[currentType.Value] =
                            values.Length > 1 && !string.IsNullOrEmpty(values[1])
                                ? CsvParser.ParsePositive<int>(values[1])
                                : type.TriggerChance;
                        currentEncounter.Types.Add(currentType.Value, []);
                    }
                    else
                    {
                        throw new SerializationException(
                            $"Undefined encounter type \"{line}\" for map '{currentEncounter.Map}'."
                        );
                    }
                }
            }
            catch (Exception e)
            {
                throw new PbsParseException($"{e}\n{fileLineData.LineReport}", e);
            }
        }

        if (currentEncounter is not null)
        {
            FinalizeEncounter(currentEncounter, encounters);
        }

        await Encounter.ImportAsync(encounters, cancellationToken);
    }

    private static void FinalizeEncounter(EncounterInfo currentEncounter, List<Encounter> encounters)
    {
        foreach (var (_, slots) in currentEncounter.Types)
        {
            for (var i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                if (slot.Chance == -1)
                    continue;

                for (var j = 0; j < slots.Count; j++)
                {
                    var otherSlot = slots[j];
                    if (i == j || otherSlot.Chance == -1)
                        continue;

                    if (
                        slot.Species != otherSlot.Species
                        || slot.MinLevel != otherSlot.MinLevel
                        || slot.MaxLevel != otherSlot.MaxLevel
                    )
                        continue;

                    slots[i] = slot with { Chance = slot.Chance + otherSlot.Chance };
                    slots[j] = slot with { Chance = -1 };
                }
            }

            for (var j = slots.Count - 1; j >= 0; j--)
            {
                var slot = slots[j];
                if (slot.Chance == -1)
                    slots.RemoveAt(j);
            }

            slots.Sort(
                (a, b) =>
                    a.Chance == b.Chance
                        ? string.Compare(a.Species.ToString(), b.Species.ToString(), StringComparison.Ordinal)
                        : b.Chance.CompareTo(a.Chance)
            );
        }

        encounters.Add(currentEncounter.ToGameData());
    }

    private static EncounterEntry ParseEntryLine(string line, EncounterInfo currentEncounter, Name currentType)
    {
        var values = line.Split(',').Select(s => s.Trim()).ToImmutableArray();
        if (values.Length < 3)
        {
            throw new SerializationException(
                $"Expected a species entry line for encounter type {EncounterType.Get(currentType)} for map {currentEncounter.Map}."
            );
        }

        var minLevel = CsvParser.ParseUnsigned<int>(values[2]);

        return new EncounterEntry(
            CsvParser.ParsePositive<int>(values[0]),
            ParseSpecies(values[1]),
            CsvParser.ParseUnsigned<int>(values[2]),
            values.Length > 3 ? CsvParser.ParseUnsigned<int>(values[3]) : minLevel
        );
    }

    private static SpeciesForm ParseSpecies(string species)
    {
        var underscoreMatch = SpeciesFormPattern.Match(species);
        if (
            underscoreMatch.Success
            && Species.TryGet(
                underscoreMatch.Groups[1].Value,
                int.Parse(underscoreMatch.Groups[2].Value),
                out var entity
            )
        )
        {
            return entity.Id;
        }

        return CsvParser.ParseDataEnum<Species, SpeciesForm>(species);
    }

    [CreateSyncVersion]
    public async Task WriteToFileAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWritingPbsFile(Path.GetFileName(_path));
        await FileUtils.WriteFileWithBackupAsync(_path, FileWrite);
        return;

        async ValueTask FileWrite(StreamWriter fileWriter)
        {
            await PbsSerializer.AddPbsHeaderToFileAsync(fileWriter);
            foreach (var encounter in Encounter.Entities)
            {
                var mapName = _mapMetadataRepository.TryGet(encounter.MapId, out var mapInfo)
                    ? $" # {mapInfo.Name}"
                    : "";
                await fileWriter.WriteLineAsync("#-------------------------------");
                if (encounter.Version > 0)
                {
                    await fileWriter.WriteLineAsync($"[{encounter.MapId:000},{encounter.Version}]{mapName}");
                }
                else
                {
                    await fileWriter.WriteLineAsync($"[{encounter.MapId:000}]{mapName}");
                }

                foreach (var (type, slots) in encounter.Types)
                {
                    await WriteEncounterBody(slots, encounter, type, fileWriter);
                }
            }
        }
    }

    private static async Task WriteEncounterBody(
        ImmutableArray<EncounterEntry> slots,
        Encounter encounter,
        Name type,
        StreamWriter fileWriter
    )
    {
        if (slots.Length == 0)
            return;

        if (encounter.StepChances.TryGetValue(type, out var stepChances) && stepChances > 0)
        {
            await fileWriter.WriteLineAsync($"{type},{stepChances}");
        }
        else
        {
            await fileWriter.WriteLineAsync(type);
        }

        foreach (var slot in slots)
        {
            if (slot.MinLevel == slot.MaxLevel)
            {
                await fileWriter.WriteLineAsync($"    {slot.Chance},{FormatSpecies(slot.Species)},{slot.MinLevel}");
            }
            else
            {
                await fileWriter.WriteLineAsync(
                    $"    {slot.Chance},{FormatSpecies(slot.Species)},{slot.MinLevel},{slot.MaxLevel}"
                );
            }
        }
    }

    private static string FormatSpecies(SpeciesForm species) =>
        species.Form == 0 ? species.Species.ToString() : $"{species.Species}_{species.Form}";

    [GeneratedRegex(@"^\d+,")]
    private static partial Regex SpeciesLine { get; }

    [GeneratedRegex(@"^\[\s*(.+)\s*\]$")]
    private static partial Regex MapIdLine { get; }

    [GeneratedRegex(@"^(\w+)_(\d+)$")]
    private static partial Regex SpeciesFormPattern { get; }
}
