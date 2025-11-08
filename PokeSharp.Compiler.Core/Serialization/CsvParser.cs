using System.Collections.Immutable;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;

namespace PokeSharp.Compiler.Core.Serialization;

public static partial class CsvParser
{
    public static IEnumerable<string> SplitCsvLine(string line)
    {
        string?[] values = line.Split(',');

        for (var i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (string.IsNullOrEmpty(value)) continue;
            
            var quoteCount = value.Count(c => c == '"');
            if (quoteCount != 0)
            {
                for (var j = i; j < values.Length - 1; j++)
                {
                    quoteCount = values[i]!.Count(c => c == '"');
                    if (quoteCount == 2 && value.StartsWith("\\\"") && values[i]!.EndsWith("\\\""))
                    {
                        values[i] = values[i]![2..^3];
                        break;
                    }

                    if (quoteCount % 2 == 0) break;


                    values[i] += "," + values[j + 1];
                    values[j + 1] = null!;
                }

                if (quoteCount != 2)
                {
                    if (value.Count(c => c == '"') == 2 && value.StartsWith("\\\"") && value.EndsWith("\\\""))
                    {
                        values[i] = values[i]![2..^3];
                    }
                }
            }

            values[i] = values[i]!.Trim();
        }
        
        return values.Where(v => v is not null)!;
    }
    
    public static object? CastCsvValue(string value, SchemaTypeData schema)
    {
        return schema.Type switch
        {
            PbsFieldType.Integer => ParseInt(value),
            PbsFieldType.UnsignedInteger => ParseUnsigned(value),
            PbsFieldType.PositiveInteger => ParsePositive(value),
            PbsFieldType.Hexadecimal => ParseHex(value),
            PbsFieldType.Float => ParseFloat(value),
            PbsFieldType.Boolean => ParseBoolean(value),
            PbsFieldType.Name => ParseName(value),
            PbsFieldType.String or PbsFieldType.UnformattedText => value,
            PbsFieldType.Symbol => ParseSymbol(value),
            PbsFieldType.Enumerable => ParseEnumField(value, schema.EnumType),
            PbsFieldType.EnumerableOrInteger => ParseEnumOrInt(value, schema.EnumType),
            _ => throw new PbsParseException($"Unknown schema '{schema}'.")
        };
    }
    

    private static long ParseInt(string value)
    {
        return long.TryParse(value, out var result)
            ? result
            : throw new PbsParseException($"Field '{value}' is not an integer.");
    }

    private static ulong ParseUnsigned(string value)
    {
        if (!ulong.TryParse(value, out var result))
        {
            throw new PbsParseException(
                $"Field '{value}' is not a positive integer or 0.");
        }

        return result;
    }

    private static ulong ParsePositive(string value)
    {
        if (!ulong.TryParse(value, out var result) || result == 0)
        {
            throw new PbsParseException(
                $"Field '{value}' is not a positive integer.");
        }

        return result;
    }

    private static ulong ParseHex(string value)
    {
        if (!ulong.TryParse(value, NumberStyles.HexNumber, null, out var result))
        {
            throw new PbsParseException(
                $"Field '{value}' is not a hexadecimal number.");
        }

        return result;
    }

    private static decimal ParseFloat(string value)
    {
        return decimal.TryParse(value, out var result) ? result : throw new PbsParseException($"Field '{value}' is not a number.");
    }

    private static bool ParseBoolean(string value)
    {
        if (TrueFormats.IsMatch(value)) return true;
        return FalseFormats.IsMatch(value) ? false : throw new PbsParseException($"Field '{value}' is not a Boolean value (true, false, 1, 0).");
    }

    [GeneratedRegex("^(?:1|TRUE|YES|Y)$", RegexOptions.IgnoreCase)]
    private static partial Regex TrueFormats { get; }

    [GeneratedRegex("^(?:0|FALSE|NO|N)$", RegexOptions.IgnoreCase)]
    private static partial Regex FalseFormats { get; }

    private static string ParseName(string value)
    {
        return NameFormat.IsMatch(value) ? value : throw new PbsParseException($"Field '{value}' is not a valid name.");
    }

    [GeneratedRegex(@"^(?![0-9])\w+$")]
    private static partial Regex NameFormat { get; }

    private static Name ParseSymbol(string value)
    {
        return ParseName(value);
    }

    private static object ParseEnumField(string value, Type? enumeration)
    {
        // TODO: For now we're just going to map to an enum type, but this will need to be more robust.

        if (enumeration is null) throw new PbsParseException("Enumeration not defined.");

        if (enumeration.IsEnum)
        {
            if (!Enum.TryParse(enumeration, value, true, out var result))
            {
                throw new PbsParseException($"Field '{value}' is not a valid value for enumeration '{enumeration}'.");
            }
            
            return result;
        }
        
        return value;
    }

    private static object? ParseEnumOrInt(string value, Type? enumeration)
    {
        return int.TryParse(value, out var result) ? result : ParseEnumField(value, enumeration);
    }

    public static object? GetCsvRecord(string record, SchemaEntry schema)
    {
        var result = new List<object?>();
        var repeat = false;
        var schemaLength = schema.TypeEntries.Length;
        switch (schema.FieldStructure)
        {
            case PbsFieldStructure.Array:
                repeat = true;
                break;
            case PbsFieldStructure.Repeating:
                schemaLength--;
                break;
            case PbsFieldStructure.Single:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(schema), schema.FieldStructure, null);
        }

        var subarrays = repeat && schema.TypeEntries.Length > 1;
        var values = SplitCsvLine(record).ToImmutableArray();
        var index = -1;
        while (true)
        {
            var parsedValues = new List<object?>();
            foreach (var typeData in schema.TypeEntries)
            {
                index++;
                if (typeData.IsOptional && string.IsNullOrEmpty(values[index]))
                {
                    parsedValues.Add(null);
                    continue;
                }

                if (typeData.Type == PbsFieldType.UnformattedText)
                {
                    parsedValues.Add(record);
                    index = values.Length;
                    break;
                }

                parsedValues.Add(CastCsvValue(values[index], typeData));
            }

            if (parsedValues.Count > 0)
            {
                if (subarrays)
                {
                    result.Add(parsedValues);
                }
                else
                {
                    result.AddRange(parsedValues);
                }
            }
            
            if (!repeat || index >= values.Length - 1) break;
        }

        return !repeat && schemaLength == 1 ? result[0] : result;
    }
}