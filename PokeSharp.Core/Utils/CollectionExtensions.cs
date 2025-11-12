namespace PokeSharp.Core.Utils;

public static class CollectionExtensions
{
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
