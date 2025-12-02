using System.Diagnostics.CodeAnalysis;
using HandlebarsDotNet;
using HandlebarsDotNet.IO;

namespace PokeSharp.Editor.SourceGenerator.Formatters;

public class EnumStringFormatter : IFormatter, IFormatterProvider
{
    public void Format<T>(T value, in EncodedTextWriter writer)
    {
        if (value is null || !value.GetType().IsEnum)
            throw new ArgumentException("Value must be an enum type");

        writer.Write(value.ToString());
    }

    public bool TryCreateFormatter(Type type, [UnscopedRef] out IFormatter? formatter)
    {
        if (!type.IsEnum)
        {
            formatter = null;
            return false;
        }

        formatter = this;
        return true;
    }
}
