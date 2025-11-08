using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Schema;

namespace PokeSharp.Compiler.Serialization;

public readonly record struct PbsParseResult(Dictionary<string, object?> LastSection, string SectionName);

public readonly record struct Section<T>(Dictionary<string, object?> Data, T Id);

public readonly record struct LineWithNumber(string Line, int LineNumber);

public partial class PbsSerializer
{
    private readonly FileLineData _fileLineData = new();
    
    public async IAsyncEnumerable<PbsParseResult> ParseFileSectionsEx(StreamReader fileReader, 
                                                                      Dictionary<string, SchemaEntry>? schema = null, 
                                                                      [EnumeratorCancellation] 
                                                                      CancellationToken cancellationToken = default)
    {
        var lineNumber = 1;
        string? sectionName = null;
        var lastSection = new Dictionary<string, object?>();
        while (await fileReader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (!line.StartsWith('#') && !string.IsNullOrWhiteSpace(line))
            {
                line = TextFormatter.PrepLine(line);

                var match = SectionHeader.Match(line);
                if (match.Success)
                {
                    if (sectionName is not null) 
                        yield return new PbsParseResult(lastSection, sectionName);
                    
                    sectionName = match.Groups[1].Value;
                    lastSection = new Dictionary<string, object?>();
                }
                else
                {
                    if (sectionName is null)
                    {
                        _fileLineData.SetLine(line, lineNumber);
                        throw new PbsFormatException($"Expected a section at the beginning of the file.\\nThis error may also occur if the file was not saved in UTF-8.\n{_fileLineData.LineReport}");
                    }

                    match = KeyValuePair.Match(line);
                    if (!match.Success)
                    {
                        _fileLineData.SetSection(sectionName, null, line);
                        throw new PbsFormatException(
                            $"Bad line syntax (expected syntax like XXX=YYY).\n{_fileLineData.LineReport}");
                    }
                    
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;

                    if (schema is not null && schema.TryGetValue(key, out var entry) &&
                        entry.TypeString.StartsWith('^'))
                    {
                        if (!lastSection.TryGetValue(key, out var existingValue) || existingValue is not IList<string> list)
                        {
                            list = new List<string>();
                            lastSection[key] = list;
                        }
                        
                        list.Add(value.TrimEnd());
                    }
                    else
                    {
                        lastSection[key] = value.TrimEnd();
                    }
                }
            }
            
            lineNumber++;
        }
        
        if (sectionName is not null)
            yield return new PbsParseResult(lastSection, sectionName);
    }

    public async IAsyncEnumerable<Section<string>> ParseFileSections(
        StreamReader fileReader, Dictionary<string, SchemaEntry>? schema = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var (section, name) in ParseFileSectionsEx(fileReader, schema, cancellationToken))
        {
            yield return new Section<string>(section, name);
        }
    }

    public async IAsyncEnumerable<Section<int>> ParseFileSectionsNumbered(
        StreamReader fileReader, Dictionary<string, SchemaEntry>? schema = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var (section, name) in ParseFileSectionsEx(fileReader, schema, cancellationToken))
        {
            if (int.TryParse(name, out var number))
            {
                yield return new Section<int>(section, number);    
            }
        }
    }

    public async IAsyncEnumerable<LineWithNumber> ParseFileLines(string filename, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var fileReader = new StreamReader(filename);
        _fileLineData.File = filename;
        var lineNumber = 1;
        while (await fileReader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (!line.StartsWith('#') && !string.IsNullOrWhiteSpace(line))
            {
                _fileLineData.SetLine(line, lineNumber);
                yield return new LineWithNumber(line, lineNumber);
            }
            
            lineNumber++;
        }
    }

    public async IAsyncEnumerable<LineWithNumber> ParsePreppedLines(string filename,
                                                                    [EnumeratorCancellation]
                                                                    CancellationToken cancellationToken = default)
    {
        using var fileReader = new StreamReader(filename);
        _fileLineData.File = filename;
        
        var lineNumber = 1;
        while (await fileReader.ReadLineAsync(cancellationToken) is { } line)
        {
            line = TextFormatter.PrepLine(line);
            if (!line.StartsWith('#') && !string.IsNullOrWhiteSpace(line))
            {
                _fileLineData.SetLine(line, lineNumber);
                yield return new LineWithNumber(line, lineNumber);
            }
            
            lineNumber++;
        }
    }
    
    [GeneratedRegex(@"^\s*\[\s*(.*)\s*\]\s*$")]
    private static partial Regex SectionHeader { get; }

    [GeneratedRegex(@"^\s*(\w+)\s*=\s*(.*)$")]
    private static partial Regex KeyValuePair { get; }

    
}