using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.Core.Serialization.Converters;
using PokeSharp.Compiler.Core.Utils;

namespace PokeSharp.Compiler.Core.Serialization;

public readonly struct ParsedData
{
    private readonly object? _data;

    public FileLineData LineData { get; }

    public ParsedData(FileLineData lineData)
    {
        _data = null;
        LineData = lineData;
    }

    public ParsedData(string data, FileLineData lineData)
    {
        _data = data;
        LineData = lineData;
    }

    public ParsedData(List<string> data, FileLineData lineData)
    {
        _data = data;
        LineData = lineData;
    }

    public bool TryGetString([NotNullWhen(true)] out string? value)
    {
        if (_data is string stringValue)
        {
            value = stringValue;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetList([NotNullWhen(true)] out List<string>? value)
    {
        if (_data is List<string> list)
        {
            value = list;
            return true;
        }

        value = null;
        return false;
    }

    public void Match(Action<string> whenString, Action<List<string>> whenList, Action whenNull)
    {
        switch (_data)
        {
            case string stringValue:
                whenString(stringValue);
                break;
            case List<string> list:
                whenList(list);
                break;
            case null:
                whenNull();
                break;
            default:
                throw new InvalidOperationException($"Unexpected type '{_data.GetType().Name}'");
        }
    }
}

public readonly record struct PbsParseResult(
    Dictionary<string, ParsedData> LastSection,
    string SectionName,
    FileLineData LineData
);

public readonly record struct Section<T>(
    Dictionary<string, ParsedData> Data,
    T Id,
    FileLineData LineData
);

public readonly record struct LineWithNumber(string Line, int LineNumber);

public readonly record struct ModelWithLine<T>(T Model, FileLineData LineData);

public partial class PbsSerializer
{
    private readonly SchemaBuilder _schemaBuilder = new();

    public List<IPbsConverter> Converters { get; } =
    [
        new LocalizingTextConverter(),
        new NumericTypeConverter(),
        new CollectionConverter(),
        new ComplexTypeConverter(),
    ];

    public IReadOnlyDictionary<string, SchemaEntry> GetSchema(Type type) =>
        _schemaBuilder.BuildSchema(type);

    public static async IAsyncEnumerable<PbsParseResult> ParseFileSectionsEx(
        StreamReader fileReader,
        FileLineData lineData,
        IReadOnlyDictionary<string, SchemaEntry>? schema = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var lineNumber = 1;
        string? sectionName = null;
        var lastSection = new Dictionary<string, ParsedData>();
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
                        yield return new PbsParseResult(
                            lastSection,
                            sectionName,
                            lineData.WithLine(sectionHeaderLine ?? "", sectionHeaderLineNumber)
                        );
                    }

                    sectionHeaderLine = line;
                    sectionHeaderLineNumber = lineNumber;
                    sectionName = match.Groups[1].Value;
                    lastSection = new Dictionary<string, ParsedData>();
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

                    if (
                        schema is not null
                        && schema.TryGetValue(key, out var entry)
                        && entry.FieldStructure == PbsFieldStructure.Repeating
                    )
                    {
                        if (
                            !lastSection.TryGetValue(key, out var existingValue)
                            || !existingValue.TryGetList(out var list)
                        )
                        {
                            list = [];
                            lastSection[key] = new ParsedData(list, lineData);
                        }

                        list.Add(value.TrimEnd());
                    }
                    else
                    {
                        lastSection[key] = new ParsedData(value.TrimEnd(), lineData);
                    }
                }
            }

            lineNumber++;
        }

        if (sectionName is null)
            yield break;

        yield return new PbsParseResult(
            lastSection,
            sectionName,
            lineData.WithLine(sectionHeaderLine ?? "", sectionHeaderLineNumber)
        );
    }

    private async IAsyncEnumerable<Section<string>> ParseFileSections(
        StreamReader fileReader,
        FileLineData lineData,
        IReadOnlyDictionary<string, SchemaEntry>? schema = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await foreach (
            var (section, name, currentLine) in ParseFileSectionsEx(
                fileReader,
                lineData,
                schema,
                cancellationToken
            )
        )
        {
            yield return new Section<string>(section, name, currentLine);
        }
    }

    public async IAsyncEnumerable<LineWithNumber> ParseFileLines(
        string filename,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        using var fileReader = new StreamReader(filename);
        var lineNumber = 1;
        while (await fileReader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (!line.StartsWith('#') && !string.IsNullOrWhiteSpace(line))
            {
                yield return new LineWithNumber(line, lineNumber);
            }

            lineNumber++;
        }
    }

    public async IAsyncEnumerable<LineWithNumber> ParsePreppedLines(
        string filename,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        using var fileReader = new StreamReader(filename);

        var lineNumber = 1;
        while (await fileReader.ReadLineAsync(cancellationToken) is { } line)
        {
            line = TextFormatter.PrepLine(line);
            if (!line.StartsWith('#') && !string.IsNullOrWhiteSpace(line))
            {
                yield return new LineWithNumber(line, lineNumber);
            }

            lineNumber++;
        }
    }

    [GeneratedRegex(@"^\s*\[\s*(.+)\s*\]\s*$")]
    public static partial Regex SectionHeader { get; }

    [GeneratedRegex(@"^\s*(\w+)\s*=\s*(.*)$")]
    public static partial Regex KeyValuePair { get; }

    public IAsyncEnumerable<ModelWithLine<T>> ReadFromFile<T>(
        string path,
        CancellationToken cancellationToken = default
    )
    {
        return ReadFromFile(path, _ => Activator.CreateInstance<T>(), cancellationToken);
    }

    public async IAsyncEnumerable<ModelWithLine<T>> ReadFromFile<T>(
        string path,
        Func<string, T> modelFactory,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var attribute = typeof(T).GetCustomAttribute<PbsDataAttribute>()!;

        if (attribute.IsOptional && !File.Exists(path))
            yield break;

        var schema = _schemaBuilder.BuildSchema(typeof(T));
        using var fileStream = new StreamReader(path);

        var initialLineData = new FileLineData(path);

        await foreach (
            var (contents, sectionName, lineData) in ParseFileSections(
                fileStream,
                initialLineData,
                schema,
                cancellationToken
            )
        )
        {
            var result = modelFactory(sectionName);
            var mappedProperties = new HashSet<string>();
            foreach (var (key, schemaEntry) in schema)
            {
                var property = typeof(T).GetProperty(schemaEntry.PropertyName);
                if (property is null)
                    throw new InvalidOperationException(
                        $"Property '{schemaEntry.PropertyName}' not found on type '{typeof(T).Name}'."
                    );

                if (key == "SectionName")
                {
                    try
                    {
                        ConversionUtils.SetValueToProperty(
                            "SectionName",
                            result,
                            property,
                            CsvParser.GetCsvRecord(sectionName, schemaEntry),
                            Converters
                        );
                    }
                    catch (Exception e)
                    {
                        throw PbsParseException.Create(e, lineData);
                    }

                    mappedProperties.Add(schemaEntry.PropertyName);
                    continue;
                }

                if (!contents.TryGetValue(key, out var value))
                    continue;

                try
                {
                    value.Match(
                        stringValue =>
                            ConversionUtils.SetValueToProperty(
                                sectionName,
                                result,
                                property,
                                CsvParser.GetCsvRecord(stringValue, schemaEntry),
                                Converters
                            ),
                        list =>
                        {
                            var propertyOutput = list.Select(item =>
                                    CsvParser.GetCsvRecord(item, schemaEntry)
                                )
                                .ToList();
                            ConversionUtils.SetValueToProperty(
                                sectionName,
                                result,
                                property,
                                propertyOutput,
                                Converters
                            );
                        },
                        () =>
                            throw new InvalidOperationException(
                                $"Property '{property.Name}' is null."
                            )
                    );
                }
                catch (Exception e)
                {
                    throw PbsParseException.Create(e, value.LineData);
                }

                mappedProperties.Add(schemaEntry.PropertyName);
            }

            var missingProperties = new List<string>();
            foreach (var property in typeof(T).GetProperties())
            {
                if (
                    property.GetCustomAttribute<RequiredMemberAttribute>() is not null
                    && !mappedProperties.Contains(property.Name)
                )
                {
                    missingProperties.Add(property.Name);
                }
            }

            if (missingProperties.Count > 0)
            {
                throw new PbsSchemaException(
                    $"The following properties are required but were not found in the file: {string.Join(", ", missingProperties)}\n{lineData.LineReport}"
                );
            }

            yield return new ModelWithLine<T>(result, lineData);
        }
    }

    public static async Task AddPbsHeaderToFile(StreamWriter fileWriter)
    {
        await fileWriter.WriteLineAsync(
            "# See the documentation on the wiki to learn how to edit this file."
        );
    }

    public async Task WritePbsFile<T>(
        string path,
        IEnumerable<T> entities,
        Func<T, string, object?> propertyGetter
    )
    {
        var attribute = typeof(T).GetCustomAttribute<PbsDataAttribute>()!;

        if (attribute.IsOptional && !File.Exists(path))
            return;

        var schema = _schemaBuilder.BuildSchema(typeof(T));

        if (!schema.TryGetValue("SectionName", out var sectionName))
        {
            throw new PbsSchemaException(
                $"Schema for type '{typeof(T).Name}' does not have a 'SectionName' field."
            );
        }

        string? backupPath = null;

        if (File.Exists(path))
        {
            backupPath = $"{path}.backup";
            File.Copy(path, backupPath, true);
        }

        try
        {
            await using var fileWriter = new StreamWriter(path, false, Encoding.UTF8);
            await AddPbsHeaderToFile(fileWriter);

            foreach (var entity in entities)
            {
                await fileWriter.WriteLineAsync("#-------------------------------");
                await fileWriter.WriteLineAsync(
                    $"[{propertyGetter(entity, sectionName.PropertyName)}]"
                );

                foreach (var (key, value) in schema)
                {
                    if (key == "SectionName")
                        continue;

                    var elementValue = propertyGetter(entity, value.PropertyName);
                    if (elementValue is null)
                        continue;

                    if (
                        value.FieldStructure == PbsFieldStructure.Repeating
                        && elementValue is IEnumerable list
                    )
                    {
                        foreach (var item in list)
                        {
                            await fileWriter.WriteAsync($"{key} = ");
                            await CsvWriter.WriteCsvRecord(item, fileWriter, value);
                            await fileWriter.WriteLineAsync();
                        }
                    }
                    else
                    {
                        await fileWriter.WriteAsync($"{key} = ");
                        await CsvWriter.WriteCsvRecord(elementValue, fileWriter, value);
                        await fileWriter.WriteLineAsync();
                    }
                }
            }
        }
        catch
        {
            if (backupPath is null || !File.Exists(backupPath))
                throw;

            try
            {
                File.Move(backupPath, path, true);
            }
            catch
            {
                File.Delete(path);
                throw;
            }

            throw;
        }
        finally
        {
            // Clean up backup file on success
            if (backupPath != null && File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }
        }
    }
}
