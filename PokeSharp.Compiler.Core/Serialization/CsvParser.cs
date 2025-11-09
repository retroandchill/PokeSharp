using System.Collections.Immutable;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Core.Data;

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
    
    public static object? CastCsvValue(string value, SchemaTypeData schema, FileLineData fileLineData)
    {
        return schema.Type switch
        {
            PbsFieldType.Integer => ParseInt(value, fileLineData),
            PbsFieldType.UnsignedInteger => ParseUnsigned(value, fileLineData),
            PbsFieldType.PositiveInteger => ParsePositive(value, fileLineData),
            PbsFieldType.Hexadecimal => ParseHex(value, fileLineData),
            PbsFieldType.Float => ParseFloat(value, fileLineData),
            PbsFieldType.Boolean => ParseBoolean(value, fileLineData),
            PbsFieldType.Name => ParseName(value, fileLineData),
            PbsFieldType.String or PbsFieldType.UnformattedText => value,
            PbsFieldType.Symbol => ParseSymbol(value, fileLineData),
            PbsFieldType.Enumerable => ParseEnumField(value, schema.EnumType, schema.AllowNone, fileLineData),
            PbsFieldType.EnumerableOrInteger => ParseEnumOrInt(value, schema.EnumType, schema.AllowNone, fileLineData),
            _ => throw new PbsParseException($"Unknown schema '{schema}'.\n{fileLineData.LineReport}")
        };
    }
    

    private static long ParseInt(string value, FileLineData fileLineData)
    {
        return long.TryParse(value, out var result)
            ? result
            : throw new PbsParseException($"Field '{value}' is not an integer.\n{fileLineData.LineReport}");
    }

    private static ulong ParseUnsigned(string value, FileLineData fileLineData)
    {
        if (!ulong.TryParse(value, out var result))
        {
            throw new PbsParseException(
                $"Field '{value}' is not a positive integer or 0.\n{fileLineData.LineReport}");
        }

        return result;
    }

    private static ulong ParsePositive(string value, FileLineData fileLineData)
    {
        if (!ulong.TryParse(value, out var result) || result == 0)
        {
            throw new PbsParseException(
                $"Field '{value}' is not a positive integer.\n{fileLineData.LineReport}");
        }

        return result;
    }

    private static ulong ParseHex(string value, FileLineData fileLineData)
    {
        if (!ulong.TryParse(value, NumberStyles.HexNumber, null, out var result))
        {
            throw new PbsParseException(
                $"Field '{value}' is not a hexadecimal number.\n{fileLineData.LineReport}");
        }

        return result;
    }

    private static decimal ParseFloat(string value, FileLineData fileLineData)
    {
        return decimal.TryParse(value, out var result) ? result : throw new PbsParseException($"Field '{value}' is not a number.\n{fileLineData.LineReport}");
    }

    private static bool ParseBoolean(string value, FileLineData fileLineData)
    {
        if (TrueFormats.IsMatch(value)) return true;
        return FalseFormats.IsMatch(value) ? false : throw new PbsParseException($"Field '{value}' is not a Boolean value (true, false, 1, 0).\n{fileLineData.LineReport}");
    }

    [GeneratedRegex("^(?:1|TRUE|YES|Y)$", RegexOptions.IgnoreCase)]
    private static partial Regex TrueFormats { get; }

    [GeneratedRegex("^(?:0|FALSE|NO|N)$", RegexOptions.IgnoreCase)]
    private static partial Regex FalseFormats { get; }

    private static string ParseName(string value, FileLineData fileLineData)
    {
        return NameFormat.IsMatch(value) ? value : throw new PbsParseException($"Field '{value}' is not a valid name.\n{fileLineData.LineReport}");
    }

    [GeneratedRegex(@"^(?![0-9])\w+$")]
    private static partial Regex NameFormat { get; }

    private static Name ParseSymbol(string value, FileLineData fileLineData)
    {
        return ParseName(value, fileLineData);
    }

    private static object? ParseEnumField(string value, Type? enumeration, bool allowNone, FileLineData fileLineData)
    {
        if (enumeration is null) throw new PbsParseException($"Enumeration not defined.\n{fileLineData.LineReport}");

        if (enumeration.IsEnum)
        {
            return Enum.TryParse(enumeration, value, true, out var result) ? result : throw new PbsParseException($"Field '{value}' is not a valid value for enumeration '{enumeration}'.\n{fileLineData.LineReport}");
        }
        
        var dataTypeInterface = enumeration.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IGameDataEntity<,>));
        if (dataTypeInterface is null) return value;
        
        var instantiatedMethod = ParseDataEnumMethod.MakeGenericMethod(enumeration, dataTypeInterface.GetGenericArguments()[0]);
        return instantiatedMethod.Invoke(null, [value, allowNone, fileLineData]);
    }

    private static readonly MethodInfo ParseDataEnumMethod =
        typeof(CsvParser).GetMethod(nameof(ParseDataEnum), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static TKey ParseDataEnum<TEntity, TKey>(string value, bool allowNone, FileLineData fileLineData)
        where TEntity : IGameDataEntity<TKey, TEntity> where TKey : notnull
    {
        var key = ConvertKey<TKey>(value, fileLineData);
        if (allowNone && IsNone(key)) return key;
        
        return TEntity.Exists(key) ? key : throw new PbsParseException($"Undefined value {value} in {typeof(TEntity)}.\n{fileLineData.LineReport}");
    }

    private static bool IsNone<TKey>(TKey key) where TKey : notnull
    {
        if (typeof(TKey) != typeof(Name)) return false;
        
        var asName = (Name)(object)key;
        return asName.IsNone;

    } 

    private static TKey ConvertKey<TKey>(string value, FileLineData fileLineData) where TKey : notnull
    {
        if (typeof(TKey).IsAssignableFrom(typeof(string)))
        { 
            return (TKey)(object)value;
        }
        
        var implicitConversion = typeof(TKey).GetMethod("op_Implicit", [typeof(string)]);
        if (implicitConversion is not null && implicitConversion.ReturnType.IsAssignableTo(typeof(TKey)))
        {
            return (TKey)implicitConversion.Invoke(null, [value])!;
        }
        
        var parseInterface = typeof(TKey).GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParsable<>) && i.GetGenericArguments()[0] == typeof(TKey));
        if (parseInterface is not null)
        {
            var parseMethod = ParseKeyMethod.MakeGenericMethod(typeof(TKey));
            return (TKey)parseMethod.Invoke(null, [value, fileLineData])!;
        }

        throw new PbsParseException($"Incorrect key type {typeof(TKey)}\n{fileLineData.LineReport}");
    }

    private static readonly MethodInfo ParseKeyMethod =
        typeof(CsvParser).GetMethod(nameof(ParseKey), BindingFlags.NonPublic | BindingFlags.Static)!;
    
    private static TKey ParseKey<TKey>(string value, FileLineData fileLineData) where TKey : IParsable<TKey>
    {
        return TKey.TryParse(value, null, out var result) ? result : throw new PbsParseException($"Could not parse {value} to type {typeof(TKey)}\n{fileLineData.LineReport}");
    }

    private static object? ParseEnumOrInt(string value, Type? enumeration, bool allowNone, FileLineData fileLineData)
    {
        return int.TryParse(value, out var result) ? result : ParseEnumField(value, enumeration, allowNone, fileLineData);
    }

    public static object? GetCsvRecord(string record, SchemaEntry schema, FileLineData fileLineData)
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
        if ((values.Length == 0 || values.Length == 1 && string.IsNullOrWhiteSpace(values[0])) && schema.FieldStructure == PbsFieldStructure.Array) return new List<object?>();
        
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

                parsedValues.Add(CastCsvValue(values[index], typeData, fileLineData));
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