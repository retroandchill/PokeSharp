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

    public static IEnumerable<IReadOnlyList<T>> EachCombination<T>(this IReadOnlyList<T> collection, int length)
    {
        if (collection.Count < length || collection.Count == 0)
            yield break;

        if (collection.Count == length)
        {
            yield return collection;
            yield break;
        }

        if (length == 1)
        {
            foreach (var item in collection)
            {
                yield return [item];
            }
            yield break;
        }

        var currentCombination = new int[length];
        var toYield = new T[length];
        for (var i = 0; i < length; i++)
        {
            currentCombination[i] = i;
        }

        while (true)
        {
            for (var i = 0; i < length; i++)
            {
                toYield[i] = collection[currentCombination[i]];
            }
            yield return toYield;
            if (!NextCombination(currentCombination, collection.Count))
            {
                yield break;
            }
        }
    }

    private static bool NextCombination(int[] combination, int length)
    {
        var i = combination.Length - 1;
        while (true)
        {
            var valid = true;
            for (var j = i; j < combination.Length; j++)
            {
                if (j == i)
                {
                    combination[j]++;
                }
                else
                {
                    combination[j] = combination[i] + (j - i);
                }

                if (combination[j] < length)
                    continue;

                valid = false;
                break;
            }

            if (valid)
                return true;
            i--;
            if (i < 0)
                break;
        }

        return false;
    }
}
