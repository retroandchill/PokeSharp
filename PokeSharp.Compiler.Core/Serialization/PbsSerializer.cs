using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.Core.Serialization.Converters;

namespace PokeSharp.Compiler.Core.Serialization;

public readonly record struct PbsParseResult(Dictionary<string, object?> LastSection, string SectionName);

public readonly record struct Section<T>(Dictionary<string, object?> Data, T Id);

public readonly record struct LineWithNumber(string Line, int LineNumber);

public partial class PbsSerializer
{
    private readonly FileLineData _fileLineData = new();
    private readonly SchemaBuilder _schemaBuilder = new();
    public List<IPbsConverter> Converters { get; } = [
        new LocalizingTextConverter(),
        new NumericTypeConverter(),
        new CollectionConverter()
    ];
    
    public async IAsyncEnumerable<PbsParseResult> ParseFileSectionsEx(StreamReader fileReader, 
                                                                      IReadOnlyDictionary<string, SchemaEntry>? schema = null, 
                                                                      [EnumeratorCancellation] 
                                                                      CancellationToken cancellationToken = default)
    {
        var lineNumber = 1;
        string? sectionName = null;
        var lastSection = new Dictionary<string, object?>();
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
                        _fileLineData.SetLine(sectionHeaderLine ?? "", sectionHeaderLineNumber);
                        yield return new PbsParseResult(lastSection, sectionName);
                    }

                    sectionHeaderLine = line;
                    sectionHeaderLineNumber = lineNumber;
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
                        entry.FieldStructure == PbsFieldStructure.Repeating)
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

        if (sectionName is null) yield break;
        
        _fileLineData.SetLine(sectionHeaderLine ?? "", sectionHeaderLineNumber);
        yield return new PbsParseResult(lastSection, sectionName);
    }

    private async IAsyncEnumerable<Section<string>> ParseFileSections(
        StreamReader fileReader, IReadOnlyDictionary<string, SchemaEntry>? schema = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var (section, name) in ParseFileSectionsEx(fileReader, schema, cancellationToken))
        {
            yield return new Section<string>(section, name);
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

    public async IAsyncEnumerable<T> ReadFromFile<T>(string path, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _fileLineData.Clear();
        var attribute = typeof(T).GetCustomAttribute<PbsDataAttribute>()!;

        if (attribute.IsOptional && !File.Exists(path)) yield break;

        var schema = _schemaBuilder.BuildSchema(typeof(T));
        using var fileStream = new StreamReader(path);
            
        _fileLineData.File = path;

        await foreach (var (contents, sectionName) in ParseFileSections(fileStream, schema, cancellationToken))
        {
            var result = Activator.CreateInstance<T>();
            var mappedProperties = new HashSet<string>();
            foreach (var (key, schemaEntry) in schema)
            {
                var property = typeof(T).GetProperty(schemaEntry.PropertyName);
                if (property is null) throw new InvalidOperationException($"Property '{schemaEntry.PropertyName}' not found on type '{typeof(T).Name}'.");
                    
                if (key == "SectionName")
                {
                    SetValueToProperty(sectionName, result, property, CsvParser.GetCsvRecord(sectionName, schemaEntry, _fileLineData));
                    mappedProperties.Add(schemaEntry.PropertyName);
                    continue;
                }

                if (!contents.TryGetValue(key, out var value)) continue;

                switch (value)
                {
                    case IList<string> list:
                        var propertyOutput = list.Select(item => CsvParser.GetCsvRecord(item, schemaEntry, _fileLineData)).ToList();
                        SetValueToProperty(sectionName, result, property, propertyOutput);
                        break;
                    case string stringValue:
                        SetValueToProperty(sectionName, result, property, CsvParser.GetCsvRecord(stringValue, schemaEntry, _fileLineData));
                        break;
                    case null:
                        throw new InvalidOperationException($"Property '{property.Name}' is null.");
                    default:
                        throw new InvalidOperationException($"Unexpected value type '{value.GetType().Name}' for property '{property.Name}'.");   
                }

                mappedProperties.Add(schemaEntry.PropertyName);
            }

            var missingProperties = new List<string>();
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.GetCustomAttribute<RequiredMemberAttribute>() is not null &&
                    !mappedProperties.Contains(property.Name))
                {
                    missingProperties.Add(property.Name);
                }
            }

            if (missingProperties.Count > 0)
            {
                throw new PbsSchemaException(
                    $"The following properties are required but were not found in the file: {string.Join(", ", missingProperties)}\n{_fileLineData.LineReport}");   
            }
                
            yield return result;
        }
    }

    private void SetValueToProperty(string sectionName, object? target, PropertyInfo property, object? value)
    {
        if (value is null) return;

        if (property.PropertyType.IsInstanceOfType(value))
        {
            property.SetValue(target, value);
            return;       
        }
        
        var converter = Converters.FirstOrDefault(c => c.CanConvert(sectionName, property, value));
        if (converter is null)
            throw new PbsParseException($"Could not find a converter for the property {property.Name}.\n{_fileLineData.LineReport}");
        
        var convertedValue = converter.Convert(sectionName, property, value);
        property.SetValue(target, convertedValue);
    }

    public static async Task AddPbsHeaderToFile(StreamWriter fileWriter)
    {
        await fileWriter.WriteLineAsync("# See the documentation on the wiki to learn how to edit this file.");
    }

    public async Task WritePbsFile<T>(string path, IEnumerable<T> entities, Func<T, string, object?> propertyGetter)
    {
        var schema = _schemaBuilder.BuildSchema(typeof(T));
        
        if (!schema.TryGetValue("SectionName", out var sectionName))
        {
            throw new PbsSchemaException(
                $"Schema for type '{typeof(T).Name}' does not have a 'SectionName' field.\n{_fileLineData.LineReport}");
        }
        
        await using var fileWriter = new StreamWriter(path, false, Encoding.UTF8);
        await AddPbsHeaderToFile(fileWriter);

        foreach (var entity in entities)
        {
            await fileWriter.WriteLineAsync("#-------------------------------");
            await fileWriter.WriteLineAsync($"[{propertyGetter(entity, sectionName.PropertyName)}]");

            foreach (var (key, value) in schema)
            {
                if (key == "SectionName") continue;
                
                var elementValue = propertyGetter(entity, value.PropertyName);
                if (elementValue is null) continue;

                if (value.FieldStructure == PbsFieldStructure.Repeating && elementValue is IEnumerable list)
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
}