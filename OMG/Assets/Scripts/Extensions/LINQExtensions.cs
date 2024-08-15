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
    }
}
