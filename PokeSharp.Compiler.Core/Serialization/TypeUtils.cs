using System.Collections;

namespace PokeSharp.Compiler.Core.Serialization;

public static class TypeUtils
{
    public static IEnumerable<object?> Flatten(this IEnumerable enumerable)
    {
        foreach (var item in enumerable)
        {
            if (item is IEnumerable enumerableItem)
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
}