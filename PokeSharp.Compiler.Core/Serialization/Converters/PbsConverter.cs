using System.Reflection;

namespace PokeSharp.Compiler.Core.Serialization.Converters;

public interface IPbsConverter
{
    public bool CanConvert(string sectionName, PropertyInfo property, object? value);
    
    public object? Convert(string sectionName, PropertyInfo property, object? value);
}

public abstract class PbsConverter<TTo, TFrom> : IPbsConverter
{
    public bool CanConvert(string sectionName, PropertyInfo property, object? value)
    {
        return property.PropertyType.IsAssignableFrom(typeof(TTo)) && value is TFrom;
    }

    public object? Convert(string sectionName, PropertyInfo property, object? value)
    {
        return value is TFrom from ? Convert(sectionName, property, from) : throw new ArgumentException("Value is not of type TFrom");
    }
    
    protected abstract TTo Convert(string sectionName, PropertyInfo property, TFrom value);
}