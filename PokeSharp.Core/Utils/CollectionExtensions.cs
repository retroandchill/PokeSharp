namespace PokeSharp.Core.Utils;

/// <summary>
/// Extension methods for collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Removes all duplicate items from the list in place.
    /// </summary>
    /// <param name="list">The list to remove elements from</param>
    /// <typeparam name="T">The type of data contained within the list.</typeparam>
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
