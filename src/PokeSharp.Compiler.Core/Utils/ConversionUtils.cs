using System.Reflection;
using PokeSharp.Compiler.Core.Serialization.Converters;

namespace PokeSharp.Compiler.Core.Utils;

public static class ConversionUtils
{
    public static void SetValueToProperty(
        string sectionName,
        object? target,
        PropertyInfo property,
        object? value,
        IReadOnlyList<IPbsConverter> converters
    )
    {
        var converted = ConvertTypeIfNecessary(sectionName, value, property.PropertyType, property, converters);
        property.SetValue(target, converted);
    }

    public static object? ConvertTypeIfNecessary(
        string sectionName,
        object? value,
        Type targetType,
        PropertyInfo property,
        IReadOnlyList<IPbsConverter> converters
    )
    {
        if (value is null)
            return null;

        if (targetType.IsInstanceOfType(value))
        {
            return value;
        }

        var converter = converters.FirstOrDefault(c => c.CanConvert(sectionName, property, value, targetType));
        if (converter is null)
            throw new InvalidOperationException($"Could not find a converter for the property {property.Name}.");

        return converter.Convert(
            sectionName,
            property,
            value,
            targetType,
            (o, t) => ConvertTypeIfNecessary(sectionName, o, t, property, converters)
        );
    }
}
