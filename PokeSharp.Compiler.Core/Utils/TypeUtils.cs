using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using PokeSharp.Abstractions;

namespace PokeSharp.Compiler.Core.Utils;

public static class TypeUtils
{
    private static readonly HashSet<Type> SimpleTypes =
    [
        typeof(bool),
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(string),
        typeof(Name),
        typeof(Text),
    ];

    public static bool IsSimpleType(Type type)
    {
        return type.IsEnum || SimpleTypes.Contains(type);
    }
}
