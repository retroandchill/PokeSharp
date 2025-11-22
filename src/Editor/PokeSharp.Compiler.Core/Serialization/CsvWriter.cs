using System.Runtime.CompilerServices;

namespace PokeSharp.Compiler.Core.Serialization;

public static class CsvWriter
{
    public static string WriteEnumOrIntegerRecord<TEnum>(TEnum record)
        where TEnum : struct, Enum
    {
        var underlyingType = Enum.GetUnderlyingType(typeof(TEnum));

        if (underlyingType == typeof(byte))
        {
            return Unsafe.As<TEnum, byte>(ref record).ToString();
        }

        if (underlyingType == typeof(short))
        {
            return Unsafe.As<TEnum, short>(ref record).ToString();
        }

        if (underlyingType == typeof(int))
        {
            return Unsafe.As<TEnum, int>(ref record).ToString();
        }

        if (underlyingType == typeof(long))
        {
            return Unsafe.As<TEnum, long>(ref record).ToString();
        }

        if (underlyingType == typeof(sbyte))
        {
            return Unsafe.As<TEnum, sbyte>(ref record).ToString();
        }

        if (underlyingType == typeof(ushort))
        {
            return Unsafe.As<TEnum, ushort>(ref record).ToString();
        }

        if (underlyingType == typeof(uint))
        {
            return Unsafe.As<TEnum, uint>(ref record).ToString();
        }

        return underlyingType == typeof(ulong) ? Unsafe.As<TEnum, ulong>(ref record).ToString() : record.ToString();
    }

    public static string WriteBoolean(bool value) => value ? "true" : "false";
}
