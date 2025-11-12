using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace PokeSharp.Compiler.Core.Utils;

public static class CollectionUtils
{
    public static IEnumerable<object?> Flatten(this IEnumerable enumerable)
    {
        foreach (var item in enumerable)
        {
            if (item is IEnumerable enumerableItem and not string)
            {
                foreach (var subItem in enumerableItem.Flatten())
                {
                    yield return subItem;
                }
            }
            else
            {
                yield return item;
            }
        }
    }

    private static readonly MethodInfo TryGetNonEnumeratedCountMethod = typeof(Enumerable).GetMethod(
        nameof(Enumerable.TryGetNonEnumeratedCount),
        BindingFlags.Public | BindingFlags.Static
    )!;

    public static bool IsEmptyEnumerable(IEnumerable enumerable)
    {
        // For other generic enumerables, use reflection
        var enumerableType = enumerable.GetType();
        var genericEnumerableInterface = enumerableType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        if (genericEnumerableInterface == null)
            return false;
        var method = TryGetNonEnumeratedCountMethod.MakeGenericMethod(
            genericEnumerableInterface.GetGenericArguments()[0]
        );
        var parameters = new object[] { enumerable, 0 };
        var result = (bool)method.Invoke(null, parameters)!;
        return result && (int)parameters[1] == 0;
    }

    public static void DistinctInPlace<T>(this IList<T> list)
    {
        if (list.Count == 0)
            return;

        var set = new HashSet<T>();
        for (var i = list.Count - 1; i >= 0; i--)
        {
            var item = list[i];
            if (!set.Add(item))
            {
                list.RemoveAt(i);
            }
        }
    }
}
