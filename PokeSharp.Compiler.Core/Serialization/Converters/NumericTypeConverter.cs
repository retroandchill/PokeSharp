using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;

namespace PokeSharp.Compiler.Core.Serialization.Converters;

public class NumericTypeConverter : IPbsConverter
{
    private static readonly ImmutableArray<Type> NumericTypes =
    [
        typeof(int),
        typeof(short),
        typeof(long),
        typeof(sbyte),
        typeof(uint),
        typeof(ulong),
        typeof(ushort),
        typeof(byte),
        typeof(float),
        typeof(double),
        typeof(decimal),
    ];

    public bool CanConvert(
        string sectionName,
        PropertyInfo property,
        object? value,
        Type targetType
    )
    {
        if (value is null)
            return false;

        return NumericTypes.Any(t => t.IsInstanceOfType(value))
            && NumericTypes.Any(t => t.IsAssignableTo(targetType));
    }

    public object? Convert(
        string sectionName,
        PropertyInfo property,
        object? value,
        Type targetType,
        Func<object?, Type, object?> convertInner
    )
    {
        if (value is null)
            return null;

        var sourceType = value.GetType();
        targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // If types are the same, no conversion needed
        if (sourceType == targetType)
            return value;

        // Check if both types are numeric
        if (!NumericTypes.Contains(sourceType) || !NumericTypes.Contains(targetType))
            throw new InvalidOperationException(
                $"Cannot convert from {sourceType.Name} to {targetType.Name}"
            );

        try
        {
            // Convert to the target type
            var convertedValue = System.Convert.ChangeType(value, targetType);

            // Verify no data loss by converting back and comparing
            if (!IsConversionLossless(value, convertedValue, sourceType))
            {
                throw new InvalidOperationException(
                    $"Conversion from {sourceType.Name} to {targetType.Name} would result in data loss. "
                        + $"Original value: {value}, Converted value: {convertedValue}"
                );
            }

            return convertedValue;
        }
        catch (OverflowException)
        {
            throw new InvalidOperationException(
                $"Conversion from {sourceType.Name} to {targetType.Name} would cause overflow. "
                    + $"Value {value} is outside the range of {targetType.Name}"
            );
        }
    }

    private static bool IsConversionLossless(
        object originalValue,
        object convertedValue,
        Type originalType
    )
    {
        try
        {
            // Convert the converted value back to the original type
            var backConverted = System.Convert.ChangeType(convertedValue, originalType);

            // For floating point types, we need special handling due to precision
            if (
                originalType == typeof(float)
                || originalType == typeof(double)
                || originalType == typeof(decimal)
            )
            {
                return IsFloatingPointEqual(originalValue, backConverted);
            }

            // For integer types, exact equality should work
            return originalValue.Equals(backConverted);
        }
        catch
        {
            // If we can't convert back, assume there's data loss
            return false;
        }
    }

    private static bool IsFloatingPointEqual(object value1, object value2)
    {
        // Handle different floating point type comparisons
        var d1 = System.Convert.ToDouble(value1);
        var d2 = System.Convert.ToDouble(value2);

        // Use a small epsilon for floating point comparison
        // or check for exact equality for special values (infinity, NaN)
        if (double.IsNaN(d1) && double.IsNaN(d2))
            return true;
        if (double.IsInfinity(d1) && double.IsInfinity(d2))
            return d1.Equals(d2);

        // For finite numbers, check if they're exactly equal
        // This works well for most integer-to-float conversions
        return d1.Equals(d2);
    }
}
