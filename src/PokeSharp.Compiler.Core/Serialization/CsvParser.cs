using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using PokeSharp.Core;
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

    public static T ParseInt<T>(string value)
        where T : struct, INumber<T>
    {
        return T.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : throw new SerializationException($"Field '{value}' is not an integer.");
    }

    public static T ParseUnsigned<T>(string value)
        where T : struct, INumber<T>, IComparisonOperators<T, int, bool>
    {
        if (!T.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) || result < 0)
        {
            throw new SerializationException($"Field '{value}' is not a positive integer or 0.");
        }

        return result;
    }

    public static T ParsePositive<T>(string value)
        where T : struct, INumber<T>, IComparisonOperators<T, int, bool>
    {
        if (!T.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) || result <= 0)
        {
            throw new SerializationException($"Field '{value}' is not a positive integer or 0.");
        }

        return result;
    }

    public static T ParseHex<T>(string value)
        where T : struct, INumber<T>, IComparisonOperators<T, int, bool>
    {
        if (!T.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result) || result < 0)
        {
            throw new SerializationException($"Field '{value}' is not a positive integer or 0.");
        }

        return result;
    }

    public static T ParseFloat<T>(string value)
        where T : struct, INumber<T>
    {
        return T.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
            ? result
            : throw new SerializationException($"Field '{value}' is not a number.");
    }

    public static bool ParseBoolean(string value)
    {
        if (TrueFormats.IsMatch(value))
            return true;
        return FalseFormats.IsMatch(value)
            ? false
            : throw new SerializationException($"Field '{value}' is not a Boolean value (true, false, 1, 0).");
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
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IGameDataEntity<,>));
        if (dataTypeInterface is null)
            return value;

        var instantiatedMethod = ParseDataEnumMethod.MakeGenericMethod(
            enumeration,
            dataTypeInterface.GetGenericArguments()[0]
        );
        return instantiatedMethod.Invoke(null, [value, allowNone]);
    }

    public static TEnum ParseEnumField<TEnum>(string value)
        where TEnum : struct, Enum
    {
        return Enum.TryParse<TEnum>(value, true, out var result)
            ? result
            : throw new SerializationException(
                $"Field '{value}' is not a valid value for enumeration '{typeof(TEnum)}'."
            );
    }

    private static readonly MethodInfo ParseDataEnumMethod = typeof(CsvParser).GetMethod(
        nameof(ParseDataEnum),
        BindingFlags.Public | BindingFlags.Static
    )!;

    public static TKey ParseDataEnum<TEntity, TKey>(string value, bool allowNone = false)
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

        var asName = Unsafe.As<TKey, Name>(ref key);
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
        if (implicitConversion is not null && implicitConversion.ReturnType.IsAssignableTo(typeof(TKey)))
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

    public static TEnum ParseEnumOrInt<TEnum>(string value)
        where TEnum : struct, Enum
    {
        if (ulong.TryParse(value, out var asUnsigned))
        {
            return ConvertToEnumType<TEnum>(asUnsigned);
        }

        return long.TryParse(value, out var asSigned)
            ? ConvertToEnumType<TEnum>((ulong)asSigned)
            : ParseEnumField<TEnum>(value);
    }

    private static TEnum ConvertToEnumType<TEnum>(ulong value)
        where TEnum : struct, Enum
    {
        var enumSize = Unsafe.SizeOf<TEnum>();
        return enumSize switch
        {
            sizeof(ulong) => Unsafe.As<ulong, TEnum>(ref value),
            sizeof(int) => Unsafe.As<int, TEnum>(ref Unsafe.As<ulong, int>(ref value)),
            sizeof(short) => Unsafe.As<short, TEnum>(ref Unsafe.As<ulong, short>(ref value)),
            sizeof(byte) => Unsafe.As<byte, TEnum>(ref Unsafe.As<ulong, byte>(ref value)),
            _ => throw new SerializationException($"Enum type {typeof(TEnum)} is not supported."),
        };
    }
}
