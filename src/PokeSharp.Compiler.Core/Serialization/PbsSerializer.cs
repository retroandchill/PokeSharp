using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using PokeSharp.Compiler.Core.Utils;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Core.Serialization;

public readonly record struct PbsKeyValueLine(string Key, string RawValue, FileLineData LineData);

public readonly record struct PbsSection(
    string SectionName,
    ImmutableArray<PbsKeyValueLine> Lines,
    FileLineData HeaderLine
);

public readonly record struct LineWithNumber(string Line, int LineNumber, FileLineData FileLineData);

public readonly record struct ModelWithLine<T>(T Model, FileLineData LineData);

public static partial class PbsSerializer
{
    [CreateSyncVersion]
    public static async IAsyncEnumerable<PbsSection> ParseFileSectionsAsync(
        StreamReader fileReader,
        FileLineData lineData,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var lineNumber = 1;
        string? sectionName = null;
        var lastSection = ImmutableArray.CreateBuilder<PbsKeyValueLine>();
        string? sectionHeaderLine = null;
        var sectionHeaderLineNumber = 0;
        while (await fileReader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (!line.StartsWith('#') && !string.IsNullOrWhiteSpace(line))
            {
                line = TextFormatter.PrepLine(line);

                var match = SectionHeader.Match(line);
                if (match.Success)
                {
                    if (sectionName is not null)
                    {
                        yield return new PbsSection(
                            sectionName,
                            lastSection.ToImmutable(),
                            lineData.WithLine(sectionHeaderLine ?? "", sectionHeaderLineNumber)
                        );
                    }

                    sectionHeaderLine = line;
                    sectionHeaderLineNumber = lineNumber;
                    sectionName = match.Groups[1].Value;
                    lastSection = ImmutableArray.CreateBuilder<PbsKeyValueLine>();
                }
                else
                {
                    if (sectionName is null)
                    {
                        lineData = lineData.WithLine(line, lineNumber);
                        throw new PbsFormatException(
                            $"Expected a section at the beginning of the file.\\nThis error may also occur if the file was not saved in UTF-8.\n{lineData.LineReport}"
                        );
                    }

                    match = KeyValuePair.Match(line);
                    if (!match.Success)
                    {
                        lineData = lineData.WithSection(sectionName, null, line);
                        throw new PbsFormatException(
                            $"Bad line syntax (expected syntax like XXX=YYY).\n{lineData.LineReport}"
                        );
                    }

                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;
                    lineData = lineData.WithSection(sectionName, key, line);

                    lastSection.Add(new PbsKeyValueLine(key, value, lineData));
                }
            }

            lineNumber++;
        }

        if (sectionName is null)
            yield break;

        yield return new PbsSection(
            sectionName,
            lastSection.ToImmutable(),
            lineData.WithLine(sectionHeaderLine ?? "", sectionHeaderLineNumber)
        );
    }

    [CreateSyncVersion]
    public static async IAsyncEnumerable<LineWithNumber> ParseFileLinesAsync(
        string filename,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var fileLineData = new FileLineData(filename);
        using var fileReader = new StreamReader(filename);
        var lineNumber = 1;
        while (await fileReader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (!line.StartsWith('#') && !string.IsNullOrWhiteSpace(line))
            {
                yield return new LineWithNumber(line, lineNumber, fileLineData.WithLine(line, lineNumber));
            }

            lineNumber++;
        }
    }

    [CreateSyncVersion]
    public static async IAsyncEnumerable<LineWithNumber> ParsePreppedLinesAsync(
        string filename,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var fileLineData = new FileLineData(filename);
        using var fileReader = new StreamReader(filename);

        var lineNumber = 1;
        while (await fileReader.ReadLineAsync(cancellationToken) is { } line)
        {
            line = TextFormatter.PrepLine(line);
            if (!line.StartsWith('#') && !string.IsNullOrWhiteSpace(line))
            {
                yield return new LineWithNumber(line, lineNumber, fileLineData.WithLine(line, lineNumber));
            }

            lineNumber++;
        }
    }

    [GeneratedRegex(@"^\s*\[\s*(.+)\s*\]\s*$")]
    private static partial Regex SectionHeader { get; }

    [GeneratedRegex(@"^\s*(\w+)\s*=\s*(.*)$")]
    private static partial Regex KeyValuePair { get; }

    [CreateSyncVersion]
    public static async IAsyncEnumerable<ModelWithLine<T>> ReadFromFileAsync<T>(
        string path,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
        where T : IPbsDataModel<T>
    {
        if (T.IsOptional && !File.Exists(path))
            yield break;

        using var fileStream = new StreamReader(path);

        var initialLineData = new FileLineData(path);

        await foreach (var section in ParseFileSectionsAsync(fileStream, initialLineData, cancellationToken))
        {
            yield return new ModelWithLine<T>(T.ParsePbsData(section), section.HeaderLine);
        }
    }

    [CreateSyncVersion]
    public static async IAsyncEnumerable<ModelWithLine<T>> ReadFromFileAsync<T>(
        string path,
        Func<string, T> factory,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
        where T : IPbsDataModel<T>
    {
        if (T.IsOptional && !File.Exists(path))
            yield break;

        using var fileStream = new StreamReader(path);

        var initialLineData = new FileLineData(path);

        await foreach (var section in ParseFileSectionsAsync(fileStream, initialLineData, cancellationToken))
        {
            yield return new ModelWithLine<T>(T.ParsePbsData(section, factory), section.HeaderLine);
        }
    }

    [CreateSyncVersion]
    public static async Task AddPbsHeaderToFileAsync(StreamWriter fileWriter)
    {
        await fileWriter.WriteLineAsync("# See the documentation on the wiki to learn how to edit this file.");
    }

    [CreateSyncVersion]
    public static async Task WritePbsFileAsync<T>(string path, IEnumerable<T> entities)
        where T : IPbsDataModel<T>
    {
        if (T.IsOptional && !File.Exists(path))
            return;

        await FileUtils.WriteFileWithBackupAsync(path, WriteAction);
        return;

        async ValueTask WriteAction(StreamWriter fileWriter)
        {
            await AddPbsHeaderToFileAsync(fileWriter);

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var entity in entities)
            {
                await fileWriter.WriteLineAsync("#-------------------------------");
                foreach (var line in entity.WritePbsData())
                {
                    await fileWriter.WriteLineAsync(line);
                }
            }
        }
    }
}
