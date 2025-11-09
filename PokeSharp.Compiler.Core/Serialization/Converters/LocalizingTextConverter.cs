using System.Reflection;
using PokeSharp.Abstractions;

namespace PokeSharp.Compiler.Core.Serialization.Converters;

public class LocalizingTextConverter : PbsConverter<Text, string>
{
    protected override Text Convert(string sectionName, PropertyInfo property, string value)
    {
        return Text.Localized(
            property.DeclaringType?.FullName ?? "Unknown",
            $"{sectionName}.{property.Name}",
            value
        );
    }
}
