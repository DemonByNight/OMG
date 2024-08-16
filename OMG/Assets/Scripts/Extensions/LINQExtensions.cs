using System.Collections.Generic;
using System.Linq;

namespace OMG
{
    public static class LINQExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IList<T> items,
        int numOfParts)
        {
            return Enumerable
            .Range(0, (items.Count + numOfParts - 1) / numOfParts)
            .Select(n => items.Skip(n * numOfParts).Take(numOfParts).ToList())
                .ToList();
        }

        public static void Swap<T>(this IList<T> items, int i1, int i2)
        {
            T buf = items[i1];
            items[i1] = items[i2];
            items[i2] = buf;
        }

        public static void AddRange<T>(this HashSet<T> items, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                if (items.Contains(item))
                    continue;

                items.Add(item);
            }
        }
    }
}
