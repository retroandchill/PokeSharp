using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
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
            if (string.IsNullOrEmpty(value))
                continue;

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

                    if (quoteCount % 2 == 0)
                        break;

                    values[i] += "," + values[j + 1];
                    values[j + 1] = null!;
                }

                if (quoteCount != 2)
                {
                    if (
                        value.Count(c => c == '"') == 2
                        && value.StartsWith("\\\"")
                        && value.EndsWith("\\\"")
                    )
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
            PbsFieldType.Enumerable => ParseEnumField(value, schema.EnumType, schema.AllowNone),
            PbsFieldType.EnumerableOrInteger => ParseEnumOrInt(
                value,
                schema.EnumType,
                schema.AllowNone
            ),
            _ => throw new SerializationException($"Unknown schema '{schema}'."),
        };
    }

    public static long ParseInt(string value)
    {
        return long.TryParse(value, out var result)
            ? result
            : throw new SerializationException($"Field '{value}' is not an integer.");
    }

    public static ulong ParseUnsigned(string value)
    {
        if (!ulong.TryParse(value, out var result))
        {
            throw new SerializationException($"Field '{value}' is not a positive integer or 0.");
        }

        return result;
    }

    public static ulong ParsePositive(string value)
    {
        if (!ulong.TryParse(value, out var result) || result == 0)
        {
            throw new SerializationException($"Field '{value}' is not a positive integer.");
        }

        return result;
    }

    public static ulong ParseHex(string value)
    {
        if (!ulong.TryParse(value, NumberStyles.HexNumber, null, out var result))
        {
            throw new SerializationException($"Field '{value}' is not a hexadecimal number.");
        }

        return result;
    }

    public static decimal ParseFloat(string value)
    {
        return decimal.TryParse(value, out var result)
            ? result
            : throw new SerializationException($"Field '{value}' is not a number.");
    }

    public static bool ParseBoolean(string value)
    {
        if (TrueFormats.IsMatch(value))
            return true;
        return FalseFormats.IsMatch(value)
            ? false
            : throw new SerializationException(
                $"Field '{value}' is not a Boolean value (true, false, 1, 0)."
            );
    }

    [GeneratedRegex("^(?:1|TRUE|YES|Y)$", RegexOptions.IgnoreCase)]
    private static partial Regex TrueFormats { get; }

    [GeneratedRegex("^(?:0|FALSE|NO|N)$", RegexOptions.IgnoreCase)]
    private static partial Regex FalseFormats { get; }

    public static string ParseName(string value)
    {
        return NameFormat.IsMatch(value)
            ? value
            : throw new SerializationException($"Field '{value}' is not a valid name.");
    }

    [GeneratedRegex(@"^(?![0-9])\w+$")]
    private static partial Regex NameFormat { get; }

    public static Name ParseSymbol(string value)
    {
        return ParseName(value);
    }

    public static object? ParseEnumField(string value, Type? enumeration, bool allowNone)
    {
        if (enumeration is null)
            throw new SerializationException("Enumeration not defined.");

        if (enumeration.IsEnum)
        {
            return Enum.TryParse(enumeration, value, true, out var result)
                ? result
                : throw new SerializationException(
                    $"Field '{value}' is not a valid value for enumeration '{enumeration}'."
                );
        }

        var dataTypeInterface = enumeration
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IGameDataEntity<,>)
            );
        if (dataTypeInterface is null)
            return value;

        var instantiatedMethod = ParseDataEnumMethod.MakeGenericMethod(
            enumeration,
            dataTypeInterface.GetGenericArguments()[0]
        );
        return instantiatedMethod.Invoke(null, [value, allowNone]);
    }

    private static readonly MethodInfo ParseDataEnumMethod = typeof(CsvParser).GetMethod(
        nameof(ParseDataEnum),
        BindingFlags.NonPublic | BindingFlags.Static
    )!;

    private static TKey ParseDataEnum<TEntity, TKey>(string value, bool allowNone)
        where TEntity : IGameDataEntity<TKey, TEntity>
        where TKey : notnull
    {
        var key = ConvertKey<TKey>(value);
        if (allowNone && IsNone(key))
            return key;

        return TEntity.Exists(key)
            ? key
            : throw new SerializationException($"Undefined value {value} in {typeof(TEntity)}.");
    }

    private static bool IsNone<TKey>(TKey key)
        where TKey : notnull
    {
        if (typeof(TKey) != typeof(Name))
            return false;

        var asName = (Name)(object)key;
        return asName.IsNone;
    }

    private static TKey ConvertKey<TKey>(string value)
        where TKey : notnull
    {
        if (typeof(TKey).IsAssignableFrom(typeof(string)))
        {
            return (TKey)(object)value;
        }

        var implicitConversion = typeof(TKey).GetMethod("op_Implicit", [typeof(string)]);
        if (
            implicitConversion is not null
            && implicitConversion.ReturnType.IsAssignableTo(typeof(TKey))
        )
        {
            return (TKey)implicitConversion.Invoke(null, [value])!;
        }

        var parseInterface = typeof(TKey)
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType
                && i.GetGenericTypeDefinition() == typeof(IParsable<>)
                && i.GetGenericArguments()[0] == typeof(TKey)
            );
        if (parseInterface is not null)
        {
            var parseMethod = ParseKeyMethod.MakeGenericMethod(typeof(TKey));
            return (TKey)parseMethod.Invoke(null, [value])!;
        }

        throw new SerializationException($"Incorrect key type {typeof(TKey)}");
    }

    private static readonly MethodInfo ParseKeyMethod = typeof(CsvParser).GetMethod(
        nameof(ParseKey),
        BindingFlags.NonPublic | BindingFlags.Static
    )!;

    private static TKey ParseKey<TKey>(string value)
        where TKey : IParsable<TKey>
    {
        return TKey.TryParse(value, null, out var result)
            ? result
            : throw new SerializationException($"Could not parse {value} to type {typeof(TKey)}");
    }

    public static object? ParseEnumOrInt(string value, Type? enumeration, bool allowNone)
    {
        return int.TryParse(value, out var result)
            ? ConvertToEnumType(result, enumeration)
            : ParseEnumField(value, enumeration, allowNone);
    }

    private static object? ConvertToEnumType(int value, Type? enumType)
    {
        if (enumType is null)
            return value;

        return Enum.IsDefined(enumType, value)
            ? Enum.ToObject(enumType, value)
            : throw new SerializationException(
                $"Value {value} is not a valid value for {enumType}."
            );
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
        if (
            (values.Length == 0 || values.Length == 1 && string.IsNullOrWhiteSpace(values[0]))
            && schema.FieldStructure == PbsFieldStructure.Array
        )
            return new List<object?>();

        var index = -1;
        while (true)
        {
            var parsedValues = new List<object?>();
            foreach (var typeData in schema.TypeEntries)
            {
                index++;
                if (
                    typeData.IsOptional
                    && (values.Length <= index || string.IsNullOrEmpty(values[index]))
                )
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

            if (!repeat || index >= values.Length - 1)
                break;
        }

        return !repeat && schemaLength == 1 ? result[0] : result;
    }
}
