using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Schema;

namespace PokeSharp.Compiler;

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
    
    public static object? CastCsvValue(string value, char schema, Type? enumeration = null)
    {
        return char.ToLower(schema) switch
        {
            SchemaValues.Integer => ParseInt(value),
            SchemaValues.UnsignedInteger => ParseUnsigned(value),
            SchemaValues.PositiveInteger => ParsePositive(value),
            SchemaValues.Hexadecimal => ParseHex(value),
            SchemaValues.Float => ParseFloat(value),
            SchemaValues.Boolean => ParseBoolean(value),
            SchemaValues.Name => ParseName(value),
            SchemaValues.String or SchemaValues.Unformatted => value,
            SchemaValues.Symbol => ParseSymbol(value),
            SchemaValues.Enum => ParseEnumField(value, enumeration),
            SchemaValues.EnumOrInteger => ParseEnumOrInt(value, enumeration),
            _ => throw new PbsParseException($"Unknown schema '{schema}'.")
        };
    }
    

    private static int ParseInt(string value)
    {
        return int.TryParse(value, out var result)
            ? result
            : throw new PbsParseException($"Field '{value}' is not an integer.");
    }

    private static int ParseUnsigned(string value)
    {
        if (!int.TryParse(value, out var result) || result < 0)
        {
            throw new PbsParseException(
                $"Field '{value}' is not a positive integer or 0.");
        }

        return result;
    }

    private static int ParsePositive(string value)
    {
        if (!int.TryParse(value, out var result) || result <= 0)
        {
            throw new PbsParseException(
                $"Field '{value}' is not a positive integer.");
        }

        return result;
    }

    private static int ParseHex(string value)
    {
        if (!int.TryParse(value, NumberStyles.HexNumber, null, out var result) || result < 0)
        {
            throw new PbsParseException(
                $"Field '{value}' is not a hexadecimal number.");
        }

        return result;
    }

    private static float ParseFloat(string value)
    {
        return float.TryParse(value, out var result) ? result : throw new PbsParseException($"Field '{value}' is not a number.");
    }

    private static bool ParseBoolean(string value)
    {
        if (TrueFormats.IsMatch(value)) return true;
        return FalseFormats.IsMatch(value) ? false : throw new PbsParseException($"Field '{value}' is not a Boolean value (true, false, 1, 0).");
    }

    [GeneratedRegex(@"^(?:1|TRUE|YES|Y)$")]
    private static partial Regex TrueFormats { get; }

    [GeneratedRegex(@"^(?:0|FALSE|NO|N)$")]
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
        var start = 0;
        var schemaLength = schema.TypeString.Length;
        switch (schema.TypeString[0])
        {
            case '*':
                repeat = true;
                start = 1;
                break;
            case '^':
                start = 1;
                schemaLength--;
                break;
        }

        var subarrays = repeat && schema.TypeString.Length - start > 1;
        var values = SplitCsvLine(record).ToImmutableArray();
        var index = -1;
        while (true)
        {
            var parsedValues = new List<object?>();
            for (var i = start; i < schema.TypeString.Length; i++)
            {
                index++;
                var schemaChar = schema.TypeString[i];
                if (char.IsUpper(schemaChar) && string.IsNullOrEmpty(values[index]))
                {
                    parsedValues.Add(null);
                    continue;
                }

                if (char.ToLower(schemaChar) == SchemaValues.Unformatted)
                {
                    parsedValues.Add(record);
                    index = values.Length;
                    break;
                }

                parsedValues.Add(CastCsvValue(values[index], schemaChar, schema.EnumTypes[i - start]));
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
            
            if (!repeat || index > values.Length - 1) break;
        }

        return !repeat || schemaLength == 1 ? result[0] : result;
    }
}