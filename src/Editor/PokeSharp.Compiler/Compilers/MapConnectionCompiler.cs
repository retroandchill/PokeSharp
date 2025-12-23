using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Runtime.Serialization;
using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Logging;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Core.Utils;
using PokeSharp.Data.Pbs;
using PokeSharp.Maps;
using Retro.ReadOnlyParams.Annotations;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public partial class MapConnectionCompiler : IPbsCompiler
{
    public int Order => 2;
    public IEnumerable<string> FileNames => [_path];

    private static readonly Dictionary<string, MapDirection> Directions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["N"] = MapDirection.North,
        ["E"] = MapDirection.East,
        ["S"] = MapDirection.South,
        ["W"] = MapDirection.West,
        ["NORTH"] = MapDirection.North,
        ["EAST"] = MapDirection.East,
        ["SOUTH"] = MapDirection.South,
        ["WEST"] = MapDirection.West,
    };

    private string _path;
    private readonly IMapMetadataRepository _mapMetadataRepository;
    private readonly ILogger<MapConnectionCompiler> _logger;
    private readonly PbsSerializer _serializer;
    private readonly IFileSystem _fileSystem;

    public MapConnectionCompiler(
        IMapMetadataRepository mapMetadataRepository,
        IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings,
        ILogger<MapConnectionCompiler> logger,
        PbsSerializer serializer,
        IFileSystem fileSystem
    )
    {
        _path = Path.Join(pbsCompileSettings.CurrentValue.PbsFileBasePath, "map_connections.txt");
        pbsCompileSettings.OnChange(x => _path = Path.Join(x.PbsFileBasePath, "map_connections.txt"));
        _mapMetadataRepository = mapMetadataRepository;
        _logger = logger;
        _serializer = serializer;
        _fileSystem = fileSystem;
    }

    [CreateSyncVersion]
    public async Task CompileAsync(CancellationToken cancellationToken = default)
    {
        var mapConnections = new List<MapConnection>();

        _logger.LogCompilingPbsFile(Path.GetFileName(_path));
        await foreach (
            var (i, foundLine) in _serializer
                .ParsePreppedLinesAsync(_path, cancellationToken)
                .Index()
                .WithCancellation(cancellationToken)
        )
        {
            var (line, _, fileLineData) = foundLine;

            try
            {
                var parsedLine = ParseLine(line, i);

                switch (parsedLine.Map1.Direction)
                {
                    case MapDirection.North:
                        if (parsedLine.Map2.Direction != MapDirection.South)
                        {
                            throw new SerializationException(
                                "North side of first map must connect with south side of second map."
                            );
                        }
                        break;
                    case MapDirection.East:
                        if (parsedLine.Map2.Direction != MapDirection.West)
                        {
                            throw new SerializationException(
                                "East side of first map must connect with west side of second map."
                            );
                        }
                        break;
                    case MapDirection.South:
                        if (parsedLine.Map2.Direction != MapDirection.North)
                        {
                            throw new SerializationException(
                                "South side of first map must connect with north side of second map."
                            );
                        }
                        break;
                    case MapDirection.West:
                        if (parsedLine.Map2.Direction != MapDirection.East)
                        {
                            throw new SerializationException(
                                "West side of first map must connect with east side of second map."
                            );
                        }
                        break;
                    default:
                        throw new SerializationException($"Invalid direction: {parsedLine.Map1.Direction}");
                }

                mapConnections.Add(parsedLine);
            }
            catch (SerializationException ex)
            {
                throw new PbsParseException($"{ex.Message}\n{fileLineData.LineReport}", ex);
            }
        }

        await MapConnection.ImportAsync(mapConnections, cancellationToken: cancellationToken);
    }

    private static MapConnection ParseLine(string line, int index)
    {
        var splitLine = CsvParser.SplitCsvLine(line).ToImmutableArray();
        if (splitLine.Length != 6)
        {
            throw new SerializationException(
                $"Invalid number of elements found lin line, expected 6, found {splitLine.Length}."
            );
        }

        return new MapConnection(
            index,
            new ConnectedMap(
                CsvParser.ParseInt<int>(splitLine[0]),
                ParseMapDirection(splitLine[1]),
                CsvParser.ParseInt<int>(splitLine[2])
            ),
            new ConnectedMap(
                CsvParser.ParseInt<int>(splitLine[3]),
                ParseMapDirection(splitLine[4]),
                CsvParser.ParseInt<int>(splitLine[5])
            )
        );
    }

    private static MapDirection ParseMapDirection(string direction)
    {
        if (int.TryParse(direction, out var asInt))
        {
            return asInt switch
            {
                2 => MapDirection.South,
                4 => MapDirection.West,
                6 => MapDirection.North,
                8 => MapDirection.East,
                _ => throw new SerializationException($"Invalid direction: {direction}"),
            };
        }

        return Directions.TryGetValue(direction, out var parsedDirection)
            ? parsedDirection
            : throw new SerializationException($"Invalid direction: {direction}");
    }

    [CreateSyncVersion]
    public async Task WriteToFileAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWritingPbsFile(Path.GetFileName(_path));
        await _fileSystem.WriteFileWithBackupAsync(_path, WriteOperations);
        return;

        async ValueTask WriteOperations(StreamWriter fileWriter)
        {
            await PbsSerializer.AddPbsHeaderToFileAsync(fileWriter);

            await fileWriter.WriteLineAsync("#-------------------------------");

            foreach (var conn in MapConnection.Entities)
            {
                if (
                    !_mapMetadataRepository.TryGet(conn.Map1.Id, out var map1)
                    || !_mapMetadataRepository.TryGet(conn.Map2.Id, out var map2)
                )
                    continue;

                var map1Name = !string.IsNullOrWhiteSpace(map1.Name) ? map1.Name.ToString() : "???";
                var map2Name = !string.IsNullOrWhiteSpace(map2.Name) ? map2.Name.ToString() : "???";
                await fileWriter.WriteLineAsync($"# {map1Name} ({conn.Map1.Id}) - {map2Name} ({conn.Map2.Id})");

                await fileWriter.WriteLineAsync(
                    $"{conn.Map1.Id},{conn.Map1.Direction.ToStringBrief()},{conn.Map1.Offset},{conn.Map2.Id},"
                        + $"{conn.Map2.Direction.ToStringBrief()},{conn.Map2.Offset}"
                );
            }
        }
    }
}
