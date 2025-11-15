using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.Core.Serialization.Converters;
using PokeSharp.Compiler.Core.Utils;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Core.Serialization;

public readonly record struct PbsKeyValueLine(string Key, string RawValue, FileLineData LineData);

public readonly record struct PbsSection(
    string SectionName,
    ImmutableArray<PbsKeyValueLine> Lines,
    FileLineData HeaderLine
);

public readonly record struct LineWithNumber(string Line, int LineNumber);

public readonly record struct ModelWithLine<T>(T Model, FileLineData LineData);

public partial class PbsSerializer
{
    private readonly SchemaBuilder _schemaBuilder = new();

    public List<IPbsConverter> Converters { get; } =
    [new LocalizingTextConverter(), new NumericTypeConverter(), new CollectionConverter(), new ComplexTypeConverter()];

    public IReadOnlyDictionary<string, SchemaEntry> GetSchema(Type type) => _schemaBuilder.BuildSchema(type);

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
    public async IAsyncEnumerable<LineWithNumber> ParseFileLinesAsync(
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

    [CreateSyncVersion]
    public async IAsyncEnumerable<LineWithNumber> ParsePreppedLinesAsync(
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

    [CreateSyncVersion]
    public IAsyncEnumerable<ModelWithLine<T>> ReadFromFileAsync<T>(
        string path,
        CancellationToken cancellationToken = default
    )
    {
        return ReadFromFileAsync(path, _ => Activator.CreateInstance<T>(), cancellationToken);
    }

    [CreateSyncVersion]
    public IAsyncEnumerable<ModelWithLine<T>> ReadFromFileAsync<T>(
        string path,
        Func<string, T> modelFactory,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
        /*
        var attribute = typeof(T).GetCustomAttribute<PbsDataAttribute>()!;

        if (attribute.IsOptional && !File.Exists(path))
            yield break;

        var schema = _schemaBuilder.BuildSchema(typeof(T));
        using var fileStream = new StreamReader(path);

        var initialLineData = new FileLineData(path);

        await foreach (
            var (contents, sectionName, lineData) in ParseFileSectionsAsync(
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
                            var propertyOutput = list.Select(item => CsvParser.GetCsvRecord(item, schemaEntry))
                                .ToList();
                            ConversionUtils.SetValueToProperty(
                                sectionName,
                                result,
                                property,
                                propertyOutput,
                                Converters
                            );
                        },
                        () => throw new InvalidOperationException($"Property '{property.Name}' is null.")
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
        */
    }

    [CreateSyncVersion]
    public static async Task AddPbsHeaderToFileAsync(StreamWriter fileWriter)
    {
        await fileWriter.WriteLineAsync("# See the documentation on the wiki to learn how to edit this file.");
    }

    [CreateSyncVersion]
    public async Task WritePbsFileAsync<T>(
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
            throw new PbsSchemaException($"Schema for type '{typeof(T).Name}' does not have a 'SectionName' field.");
        }

        await FileUtils.WriteFileWithBackupAsync(path, WriteAction);
        return;

        async ValueTask WriteAction(StreamWriter fileWriter)
        {
            await AddPbsHeaderToFileAsync(fileWriter);

            foreach (var entity in entities)
            {
                await fileWriter.WriteLineAsync("#-------------------------------");
                await fileWriter.WriteLineAsync($"[{propertyGetter(entity, sectionName.PropertyName)}]");

                foreach (var (key, value) in schema)
                {
                    if (key == "SectionName")
                        continue;

                    var elementValue = propertyGetter(entity, value.PropertyName);
                    if (elementValue is null)
                        continue;

                    if (value.FieldStructure == PbsFieldStructure.Repeating && elementValue is IEnumerable list)
                    {
                        foreach (var item in list)
                        {
                            await fileWriter.WriteAsync($"{key} = ");
                            await CsvWriter.WriteCsvRecordAsync(item, fileWriter, value);
                            await fileWriter.WriteLineAsync();
                        }
                    }
                    else
                    {
                        await fileWriter.WriteAsync($"{key} = ");
                        await CsvWriter.WriteCsvRecordAsync(elementValue, fileWriter, value);
                        await fileWriter.WriteLineAsync();
                    }
                }
            }
        }
    }
}
