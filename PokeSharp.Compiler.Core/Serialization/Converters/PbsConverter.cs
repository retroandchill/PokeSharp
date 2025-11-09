using System.Reflection;

namespace PokeSharp.Compiler.Core.Serialization.Converters;

public interface IPbsConverter
{
    public bool CanConvert(
        string sectionName,
        PropertyInfo property,
        object? value,
        Type targetType
    );

    public object? Convert(
        string sectionName,
        PropertyInfo property,
        object? value,
        Type targetType,
        Func<object?, Type, object?> convertInner
    );
}

public abstract class PbsConverter<TTo, TFrom> : IPbsConverter
{
    public bool CanConvert(
        string sectionName,
        PropertyInfo property,
        object? value,
        Type targetType
    )
    {
        return targetType.IsAssignableFrom(typeof(TTo)) && value is TFrom;
    }

    public object? Convert(
        string sectionName,
        PropertyInfo property,
        object? value,
        Type targetType,
        Func<object?, Type, object?> convertInner
    )
    {
        return value is TFrom from
            ? Convert(sectionName, property, from)
            : throw new ArgumentException("Value is not of type TFrom");
    }

    protected abstract TTo Convert(string sectionName, PropertyInfo property, TFrom value);
}
