using System.Collections;
using System.Reflection;
using PokeSharp.Compiler.Core.Utils;

namespace PokeSharp.Compiler.Core.Serialization.Converters;

public class ComplexTypeConverter : IPbsConverter
{
    public bool CanConvert(
        string sectionName,
        PropertyInfo property,
        object? value,
        Type targetType
    )
    {
        return !TypeUtils.IsSimpleType(targetType)
            && value is IList list
            && targetType.GetConstructors().Any(c => c.GetParameters().Length == list.Count);
    }

    public object Convert(
        string sectionName,
        PropertyInfo property,
        object? value,
        Type targetType,
        Func<object?, Type, object?> convertInner
    )
    {
        ArgumentNullException.ThrowIfNull(value);
        var arguments = (IList)value;
        var targetConstructor = targetType
            .GetConstructors()
            .First(c => c.GetParameters().Length == arguments.Count);

        var parameterList = targetConstructor.GetParameters();
        for (var i = 0; i < arguments.Count; i++)
        {
            arguments[i] = convertInner(arguments[i], parameterList[i].ParameterType);
        }

        return targetConstructor.Invoke(arguments.Cast<object?>().ToArray());
    }
}
