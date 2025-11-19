using Injectio.Attributes;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Core.Utils;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public partial class MetadataCompiler : IPbsCompiler
{
    public int Order => 17;
    private string _path;
    public IEnumerable<string> FileNames => [_path];

    public MetadataCompiler(IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings)
    {
        _path = Path.Join(pbsCompileSettings.CurrentValue.PbsFileBasePath, $"{Metadata.DataPath}.txt");
        pbsCompileSettings.OnChange(x => _path = Path.Join(x.PbsFileBasePath, $"{Metadata.DataPath}.txt"));
    }

    [CreateSyncVersion]
    public async Task CompileAsync(CancellationToken cancellationToken = default)
    {
        Metadata? metadata = null;
        var playerMetadata = new OrderedDictionary<int, PlayerMetadata>();
        var fileLineData = new FileLineData(_path);
        using var streamReader = new StreamReader(_path);
        await foreach (
            var section in PbsSerializer.ParseFileSectionsAsync(streamReader, fileLineData, cancellationToken)
        )
        {
            if (section.SectionName == "0")
            {
                metadata = ParseSection(section);
            }
            else
            {
                var newPlayerMetadata = ParsePlayerSection(section);
                playerMetadata.Add(newPlayerMetadata.Id, newPlayerMetadata);
            }
        }

        if (metadata is null)
        {
            throw new PbsParseException($"Global metadata is not defined in {_path} but should be.");
        }

        if (!playerMetadata.ContainsKey(1))
        {
            throw new PbsParseException(
                $"Player metadata for player character 1 is not defined in {_path} but should be."
            );
        }

        await Metadata.ImportAsync([metadata], cancellationToken);
        await PlayerMetadata.ImportAsync(playerMetadata.Values, cancellationToken);
    }

    private static Metadata ParseSection(PbsSection section)
    {
        return MetadataInfo.ParsePbsData(section).ToGameData();
    }

    private static PlayerMetadata ParsePlayerSection(PbsSection section)
    {
        return PlayerMetadataInfo.ParsePbsData(section).ToGameData();
    }

    [CreateSyncVersion]
    public async Task WriteToFileAsync(CancellationToken cancellationToken = default)
    {
        await FileUtils.WriteFileWithBackupAsync(_path, WriteTask);
        return;

        async ValueTask WriteTask(StreamWriter fileWriter)
        {
            await PbsSerializer.AddPbsHeaderToFileAsync(fileWriter);
            await fileWriter.WriteLineAsync("#-------------------------------");
            foreach (var line in Metadata.Instance.ToDto().WritePbsData())
            {
                await fileWriter.WriteLineAsync(line);
            }

            foreach (var player in PlayerMetadata.Entities.Select(player => player.ToDto()))
            {
                await fileWriter.WriteLineAsync("#-------------------------------");
                foreach (var line in player.WritePbsData())
                {
                    await fileWriter.WriteLineAsync(line);
                }
            }
        }
    }
}
